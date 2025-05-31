using System;
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

    public TwilioSmsService(IOptionsSnapshot<AppSettings> appSettings)
    {
        _twilioSettings = appSettings.Value.TwilioConfig;
        TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthToken);
    }

    public async Task<Result> SendVerificationCodeAsync(string phoneNumber)
    {
        try
        {
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

    public async Task<Result> VerifyCodeAsync(string phoneNumber, string code)
    {
        try
        {
            var verificationCheck = await VerificationCheckResource.CreateAsync(
                to: phoneNumber,
                code: code,
                pathServiceSid: _twilioSettings.VerifyServiceSid
            );

            if (verificationCheck.Status == "approved")
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
            return Result.Failure(
                Error.ExternalServiceError("Failed to verify code."),
                ErrorType.InternalServerError
            );
        }
    }
}
