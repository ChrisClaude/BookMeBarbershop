using System;
using BookMe.Application.Commands.Abstractions;
using BookMe.Application.Common.Dtos;

namespace BookMe.Application.Commands;

public class GetOrCreateUserCommand : AuthenticatedRequest<UserDto>
{
    public string Email { get; set; }

    public GetOrCreateUserCommand(string email)
    {

    }
}
