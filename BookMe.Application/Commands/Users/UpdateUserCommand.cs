using BookMe.Application.Commands.Abstractions;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;

namespace BookMe.Application.Commands.Users;

public class UpdateUserCommand : AuthenticatedRequest<Result<UserDto>>
{
    public string Name { get; set; }
    public string Surname { get; set; }

    public UpdateUserCommand(string name, string surname)
    {
        Name = name;
        Surname = surname;
    }
}
