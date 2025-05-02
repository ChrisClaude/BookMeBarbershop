using System;
using BookMe.Application.Enums;

namespace BookMe.Application.Common.Dtos;

public record UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public IEnumerable<string> Roles { get; set; }

    public bool IsAdmin => Roles.Contains(RoleName.Admin);

    public bool IsCustomer => Roles.Contains(RoleName.Customer);
}
