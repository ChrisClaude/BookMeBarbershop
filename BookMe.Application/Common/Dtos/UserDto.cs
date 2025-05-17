using System;
using System.Text.Json.Serialization;
using BookMe.Application.Common.Dtos.Users;
using BookMe.Application.Enums;
using Microsoft.OpenApi.Extensions;

namespace BookMe.Application.Common.Dtos;

public record UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public IEnumerable<UserRoleDto> Roles { get; set; }
}
