using BookMe.Application.Commands.Users;
using BookMe.Application.Common;
using BookMe.Application.Interfaces;
using MediatR;

namespace BookMe.Application.Services;

public class UserService(IMediator mediator, ITwilioSmsService twilioSmsService) : IUserService
{
    public async Task<Result> VerifyPhoneNumberAsync(string phoneNumber, string code)
    {
        var verificationResult = await twilioSmsService.VerifyCodeAsync(phoneNumber, code);

        if (verificationResult.IsFailure)
        {
            return verificationResult;
        }

        var updateResult = await mediator.Send(new UpdateUserPhoneNumberCommand(phoneNumber, code));

        return updateResult;
    }
}
