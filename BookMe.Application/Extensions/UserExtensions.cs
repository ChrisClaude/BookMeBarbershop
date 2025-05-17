using System;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Enums;

namespace BookMe.Application.Extensions;

public static class UserDtoExtensions
{
    public static bool IsAdmin(this UserDto user)
    {
        return user.Roles.Any(x => x.Role.Name == RoleName.ADMIN);
    }

    public static bool IsCustomer(this UserDto user)
    {
        return user.Roles.Any(x => x.Role.Name == RoleName.CUSTOMER);
    }
}
