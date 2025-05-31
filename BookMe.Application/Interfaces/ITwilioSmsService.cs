using BookMe.Application.Common;

namespace BookMe.Application.Interfaces;

public interface ITwilioSmsService
{
    Task<Result> SendVerificationCodeAsync(string phoneNumber);

    Task<Result> VerifyCodeAsync(string phoneNumber, string code);
}
