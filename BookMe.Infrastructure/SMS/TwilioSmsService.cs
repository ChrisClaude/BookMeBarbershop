using System.Collections.Concurrent;
using BookMe.Application.Caching;
using BookMe.Application.Common;
using BookMe.Application.Common.Errors;
using BookMe.Application.Configurations;
using BookMe.Application.Enums;
using BookMe.Application.Interfaces;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Wrap;
using Serilog;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Verify.V2.Service;

namespace BookMe.Infrastructure.SMS;

public class TwilioSmsService : ITwilioSmsService
{
    private readonly TwilioConfig _twilioConfig;
    private readonly CacheConfig _cacheConfig;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly ConcurrentDictionary<string, DateTime> _lastVerificationAttempts = new();
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
    private readonly AsyncPolicyWrap _resilientPolicy;
    private readonly ICacheManager _cacheManager;

    public TwilioSmsService(ICacheManager cacheManager, IOptionsSnapshot<AppSettings> appSettings)
    {
        _twilioConfig = appSettings.Value.TwilioConfig;
        _cacheConfig = appSettings.Value.CacheConfig;
        TwilioClient.Init(_twilioConfig.AccountSid, _twilioConfig.AuthToken);
        _cacheManager = cacheManager;

        _circuitBreakerPolicy = Policy
            .Handle<ApiException>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromMinutes(1),
                onBreak: (ex, breakDuration) =>
                {
                    Log.Warning(
                        "Circuit breaker opened for {breakDuration} due to: {exception}",
                        breakDuration,
                        ex.Message
                    );
                },
                onReset: () => Log.Information("Circuit breaker reset, calls allowed again"),
                onHalfOpen: () => Log.Information("Circuit breaker half-open, next call is trial")
            );

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

        _resilientPolicy = Policy.WrapAsync(_circuitBreakerPolicy, _retryPolicy);
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
                        _twilioConfig.MinSecondsBetweenRequests
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

        return await _resilientPolicy.ExecuteAsync(async () =>
        {
            try
            {
                Log.Information("Sending verification code to {phoneNumber}", phoneNumber);

                var verification = await VerificationResource.CreateAsync(
                    to: phoneNumber,
                    channel: "sms",
                    pathServiceSid: _twilioConfig.VerifyServiceSid
                );

                if (verification.Status == "failed")
                {
                    Log.Error(
                        "Failed to send verification code. This could be a validation error. Phone number: {phoneNumber}. Status: {status} {@verification}",
                        phoneNumber,
                        verification.Status,
                        verification
                    );
                    return Result.Failure(
                        Error.BadRequest(
                            $"Failed to send verification code. Maybe a phone number validation error. Status: {verification.Status}"
                        ),
                        ErrorType.BadRequest
                    );
                }

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
        return await _resilientPolicy.ExecuteAsync(async () =>
        {
            var cacheKey = new CacheKey(
                $"{CacheKeyConstants.TWILIO_VERIFICATION_RESULT}_{phoneNumber}",
                _cacheConfig
            );

            if (await _cacheManager.GetAsync<string>(cacheKey, out _))
            {
                Log.Information(
                    "Getting verification result from cache. Code verified successfully for {phoneNumber}",
                    phoneNumber
                );
                return Result.Success();
            }

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
                    pathServiceSid: _twilioConfig.VerifyServiceSid
                );

                if (verificationCheck.Status == "failed")
                {
                    Log.Error(
                        "Failed to verify code. This could be a validation error. Phone number: {phoneNumber} Code: {code}. Status: {status} {@verificationCheck}",
                        phoneNumber,
                        code,
                        verificationCheck.Status,
                        verificationCheck
                    );
                    return Result.Failure(
                        Error.BadRequest(
                            $"Failed to verify code. Maybe a phone number or code validation error. Status: {verificationCheck.Status}"
                        ),
                        ErrorType.BadRequest
                    );
                }

                if (verificationCheck.Status != "approved")
                {
                    return Result.Failure(
                        Error.ExternalServiceError("Invalid verification code. Please try again."),
                        ErrorType.BadRequest
                    );
                }

                Log.Information("Code verified successfully for {phoneNumber}", phoneNumber);

                await _cacheManager.AddAsync(cacheKey, verificationCheck.Sid);

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
        return timeSinceLastAttempt.TotalSeconds < _twilioConfig.MinSecondsBetweenRequests;
    }

    private bool HasVerificationBeenSent(string phoneNumber, out DateTime lastAttempt)
    {
        return _lastVerificationAttempts.TryGetValue(phoneNumber, out lastAttempt);
    }
}
