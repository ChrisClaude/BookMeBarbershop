using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using BookMe.Application.Common;
using BookMe.Application.Common.Errors;
using BookMe.Application.Configurations;
using BookMe.Application.Interfaces;
using Microsoft.Extensions.Options;
using Serilog;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Verify.V2.Service;

namespace BookMe.Infrastructure.SMS;

public class TwilioSmsService : ITwilioSmsService
{
    private readonly TwilioConfig _twilioSettings;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly ConcurrentDictionary<string, DateTime> _lastVerificationAttempts = new();

    public TwilioSmsService(IOptionsSnapshot<AppSettings> appSettings)
    {
        _twilioSettings = appSettings.Value.TwilioConfig;
        TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthToken);
    }

    public async Task<Result> SendVerificationCodeAsync(string phoneNumber)
    {
        try
        {
            await _semaphore.WaitAsync();
            try
            {
                if (HasVerificationBeenSent(phoneNumber, out var lastAttempt))
                {
                    var timeSinceLastAttempt = DateTime.UtcNow - lastAttempt;
                    if (IsMinWaitingTimeNotElapsed(timeSinceLastAttempt))
                    {
                        var secondsToWait =
                            _twilioSettings.MinSecondsBetweenRequests
                            - (int)timeSinceLastAttempt.TotalSeconds;

                        Log.Error(
                            "Too many requests for {phoneNumber}. Please wait {secondsToWait} seconds before requesting another code.",
                            phoneNumber,
                            secondsToWait
                        );

                        return Result.Failure(
                            Error.TooManyRequestsError(
                                $"Please wait {secondsToWait} seconds before requesting another code."
                            ),
                            ErrorType.TooManyRequests
                        );
                    }
                }

                _lastVerificationAttempts[phoneNumber] = DateTime.UtcNow;
            }
            finally
            {
                _semaphore.Release();
            }

            Log.Information("Sending verification code to {phoneNumber}", phoneNumber);

            var verification = await VerificationResource.CreateAsync(
                to: phoneNumber,
                channel: "sms",
                pathServiceSid: _twilioSettings.VerifyServiceSid
            );

            if (verification.Status != "pending")
            {
                Log.Error(
                    "Failed to send verification code. Status: {status} {@verification}",
                    verification.Status,
                    verification
                );
                return Result.Failure(
                    Error.ExternalServiceError(
                        $"Failed to send verification code. Status: {verification.Status}"
                    ),
                    ErrorType.InternalServerError
                );
            }

            Log.Information("Verification code sent. {@verification}", verification);

            return Result.Success();
        }
        catch (ApiException e)
        {
            Log.Error("Failed to send verification code. {@exception}", e);
            return Result.Failure(
                Error.ExternalServiceError("Failed to send verification code."),
                ErrorType.InternalServerError
            );
        }
    }

    private bool IsMinWaitingTimeNotElapsed(TimeSpan timeSinceLastAttempt)
    {
        return timeSinceLastAttempt.TotalSeconds < _twilioSettings.MinSecondsBetweenRequests;
    }

    private bool HasVerificationBeenSent(string phoneNumber, out DateTime lastAttempt)
    {
        return _lastVerificationAttempts.TryGetValue(phoneNumber, out lastAttempt);
    }

    public async Task<Result> VerifyCodeAsync(string phoneNumber, string code)
    {
        try
        {
            Log.Information("Verifying code for {phoneNumber}", phoneNumber);

            var verificationCheck = await VerificationCheckResource.CreateAsync(
                to: phoneNumber,
                code: code,
                pathServiceSid: _twilioSettings.VerifyServiceSid
            );

            if (verificationCheck.Status != "approved")
            {
                return Result.Failure(
                    Error.ExternalServiceError("Invalid verification code. Please try again."),
                    ErrorType.BadRequest
                );
            }

            return Result.Success();
        }
        catch (ApiException e)
        {
            // TODO: Implement retry logic for network failures
            Log.Error("Failed to verify code. {@exception}", e);
            return Result.Failure(
                Error.ExternalServiceError("Failed to verify code."),
                ErrorType.InternalServerError
            );
        }
    }
}
