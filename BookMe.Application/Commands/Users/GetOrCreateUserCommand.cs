using System;
using BookMe.Application.Commands.Abstractions;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;
using MediatR;

namespace BookMe.Application.Commands;

public class GetOrCreateUserCommand : IRequest<Result<UserDto>>
{
    public string Email { get; set; }

    public GetOrCreateUserCommand(string email)
    {

    }
}
