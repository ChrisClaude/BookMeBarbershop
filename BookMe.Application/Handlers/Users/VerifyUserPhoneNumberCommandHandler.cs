using BookMe.Application.Commands.Users;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Common.Errors;
using BookMe.Application.Entities;
using BookMe.Application.Enums;
using BookMe.Application.Interfaces;
using BookMe.Application.Mappings;
using MediatR;

namespace BookMe.Application.Handlers;

/// <summary>
/// Handle the user phone number verification. If the verification is successful, the user is updated with the IsPhoneNumberVerified flag.
/// </summary>
public class VerifyUserPhoneNumberCommandHandler(
    IRepository<User> repository,
    ITwilioSmsService twilioSmsService
) : IRequestHandler<VerifyUserPhoneNumberCommand, Result>
{
    public async Task<Result> Handle(
        VerifyUserPhoneNumberCommand request,
        CancellationToken cancellationToken
    )
    {
        var verificationResult = await twilioSmsService.VerifyCodeAsync(
            request.PhoneNumber,
            request.Code
        );

        if (verificationResult.IsFailure)
        {
            return verificationResult;
        }

        // The phone number is verified, we can update the user

        var user = await repository.GetByIdAsync(request.UserDto.Id);
        if (user == null)
        {
            return Result.Failure(Error.NotFound("User not found"), ErrorType.NotFound);
        }

        user.PhoneNumber = request.PhoneNumber;
        user.IsPhoneNumberVerified = true;

        await repository.UpdateAsync(user, false);

        return Result<UserDto>.Success(user.MapToDto());
    }
}
