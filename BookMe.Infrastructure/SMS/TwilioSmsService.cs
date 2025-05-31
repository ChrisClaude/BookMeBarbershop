using System.Collections.Concurrent;
using BookMe.Application.Common;
using BookMe.Application.Common.Errors;
using BookMe.Application.Configurations;
using BookMe.Application.Interfaces;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
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
    private readonly AsyncRetryPolicy _retryPolicy;

    public TwilioSmsService(IOptionsSnapshot<AppSettings> appSettings)
    {
        _twilioSettings = appSettings.Value.TwilioConfig;
        TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthToken);

        _retryPolicy = Policy
            .Handle<ApiException>(ex =>
                // Twilio API exceptions that indicate a transient error
                ex.Status == 500
                || // Internal Server Error
                ex.Status == 503
                || // Service Unavailable
                ex.Status == 429
                || // Too Many Requests (if Twilio's own rate limit is hit before ours)
                ex.Code == 20001
                || // General Twilio error, sometimes transient
                ex.Code == 20002 // Authentication error, but can sometimes be transient with bad network
            )
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Exponential backoff: 1s, 2s, 4s
                (exception, timeSpan, retryCount, context) =>
                {
                    Log.Warning(
                        "Twilio API call failed with {exceptionMessage}. Retrying in {totalSeconds}s ({retryCount}/{@context}).",
                        exception.Message,
                        timeSpan.TotalSeconds,
                        retryCount,
                        context
                    );
                }
            );
    }

    public async Task<Result> SendVerificationCodeAsync(string phoneNumber)
    {
        var validationResult = PhoneValidatorHelper.IsValidPhoneNumber(phoneNumber);
        if (validationResult.IsFailure)
        {
            return validationResult;
        }

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

        return await _retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
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
                throw; // rethrow to be caught by the retry policy
            }
        });
    }

    public async Task<Result> VerifyCodeAsync(string phoneNumber, string code)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var validationResult = PhoneValidatorHelper.IsValidPhoneNumber(phoneNumber);
            if (validationResult.IsFailure)
            {
                return validationResult;
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                return Result.Failure(Error.BadRequest("Code is required."), ErrorType.BadRequest);
            }

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
                Log.Error("Failed to verify code. {@exception}", e);
                throw; // rethrow to be caught by the retry policy
            }
        });
    }

    private bool IsMinWaitingTimeNotElapsed(TimeSpan timeSinceLastAttempt)
    {
        return timeSinceLastAttempt.TotalSeconds < _twilioSettings.MinSecondsBetweenRequests;
    }

    private bool HasVerificationBeenSent(string phoneNumber, out DateTime lastAttempt)
    {
        return _lastVerificationAttempts.TryGetValue(phoneNumber, out lastAttempt);
    }
}
