using System;
using BookMe.Application.Commands;
using BookMe.Application.Commands.Users;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Entities;
using BookMe.Application.Interfaces;
using BookMe.Application.Mappings;
using MediatR;

namespace BookMe.Application.Handlers;

public class GetOrCreateUserCommandHandler(IRepository<User> repository, IMediator mediator) : IRequestHandler<GetOrCreateUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetOrCreateUserCommand request, CancellationToken cancellationToken)
    {
        var users = await repository.GetAllAsync(query => query.Where(x => x.Email == request.Email),
        [nameof(User.UserRoles), $"{nameof(User.UserRoles)}.{nameof(UserRole.Role)}"]
        );

        var user = users.FirstOrDefault();

        if (user != null)
        {
            return Result<UserDto>.Success(user.MapToDto());
        }

        var newUser = new User
        {
            Email = request.Email,
            Name = request.Email.Split("@")[0],
            Surname = request.Email.Split("@")[0],
        };

        var createUserResult = await mediator.Send(new CreateUserCommand(newUser.Name, newUser.Surname, newUser.Email, newUser.PhoneNumber),
        cancellationToken);

        return createUserResult;
    }
}
