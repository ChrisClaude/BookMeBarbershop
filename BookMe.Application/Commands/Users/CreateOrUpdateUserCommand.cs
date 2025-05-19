using System;
using BookMe.Application.Commands.Abstractions;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;
using MediatR;

namespace BookMe.Application.Commands.Users;

public class CreateUserCommand : IRequest<Result<UserDto>>
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    public CreateUserCommand(string name, string surname, string email, string phoneNumber = null)
    {
        Name = name;
        Surname = surname;
        Email = email;
        PhoneNumber = phoneNumber;
    }
}
