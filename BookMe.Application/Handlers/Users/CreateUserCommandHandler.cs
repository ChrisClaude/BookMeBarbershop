using BookMe.Application.Commands.Users;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Entities;
using BookMe.Application.Enums;
using BookMe.Application.Interfaces;
using BookMe.Application.Mappings;
using MediatR;

namespace BookMe.Application.Handlers;

/// <summary>
/// Handle the creation of a new user. The user is created with a default role of a customer.
/// </summary>
public class CreateUserCommandHandler(IRepository<User> repository)
    : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = new User
        {
            Name = request.Name,
            Surname = request.Surname,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            UserRoles = new List<UserRole>() { new() { RoleId = DefaultRoles.CustomerId } },
        };

        await repository.InsertAsync(user, false);

        var userWithRole = await repository.GetByIdAsync(
            user.Id,
            new string[]
            {
                nameof(User.UserRoles),
                $"{nameof(User.UserRoles)}.{nameof(UserRole.Role)}",
            }
        );

        return Result<UserDto>.Success(userWithRole.MapToDto());
    }
}
