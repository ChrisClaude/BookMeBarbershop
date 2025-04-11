using System;
using AutoMapper;
using BookMe.Application.Commands.Users;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Common.Errors;
using BookMe.Application.Entities;
using BookMe.Application.Interfaces;
using BookMe.Application.Validators;
using MediatR;

namespace BookMe.Application.Handlers;

public class CreateUserCommandHandler(IRepository<User> repository, IMapper mapper) : IRequestHandler<CreateOrUpdateUserCommand, Result<UserDto>>
{

    public async Task<Result<UserDto>> Handle(CreateOrUpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Name = request.Name,
            Surname = request.Surname,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };

        await repository.InsertAsync(user);
        var createdUser = mapper.Map<UserDto>(user);

        return Result<UserDto>.Success(createdUser);
    }
}
