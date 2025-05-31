using System.Linq.Expressions;
using BookMe.Application.Commands.Users;
using BookMe.Application.Common;
using BookMe.Application.Common.Errors;
using BookMe.Application.Entities;
using BookMe.Application.Interfaces;
using MediatR;

namespace BookMe.Application.Handlers;

/// <summary>
/// Handle the user phone number verification. If the verification is successful, the user is updated with the IsPhoneNumberVerified flag.
/// </summary>
public class UpdateUserPhoneNumberCommandHandler(IRepository<User> repository)
    : IRequestHandler<UpdateUserPhoneNumberCommand, Result>
{
    public async Task<Result> Handle(
        UpdateUserPhoneNumberCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await repository.GetByIdAsync(request.UserDto.Id);
        if (user == null)
        {
            return Result.Failure(Error.NotFound("User not found"), ErrorType.NotFound);
        }

        user.PhoneNumber = request.PhoneNumber;
        user.IsPhoneNumberVerified = true;

        await repository.UpdateSpecificPropertiesAsync(
            user.Id,
            new Dictionary<Expression<Func<User, object>>, object>
            {
                { x => x.PhoneNumber, request.PhoneNumber },
                { x => x.IsPhoneNumberVerified, true },
            },
            false
        );

        return Result.Success();
    }
}
