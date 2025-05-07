using System;
using BookMe.Application.Common.Dtos.Users;
using BookMe.Application.Enums;

namespace BookMe.Application.Common.Dtos;

public record UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public IEnumerable<UserRoleDto> Roles { get; set; }

    public bool IsAdmin => Roles.Any(x => x.Role.Name == RoleName.ADMIN);

    public bool IsCustomer => Roles.Any(x => x.Role.Name == RoleName.CUSTOMER);
}
