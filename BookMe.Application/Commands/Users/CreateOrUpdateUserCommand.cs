using System;
using BookMe.Application.Commands.Abstractions;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;

namespace BookMe.Application.Commands.Users;

public class CreateOrUpdateUserCommand : AuthenticatedRequest<Result<UserDto>>
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    public CreateOrUpdateUserCommand(string name, string surname, string email, string phoneNumber = null)
    {
        Name = name;
        Surname = surname;
        Email = email;
        PhoneNumber = phoneNumber;
    }
}
