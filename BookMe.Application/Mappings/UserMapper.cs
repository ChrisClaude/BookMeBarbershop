using System;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Entities;

namespace BookMe.Application.Mappings;

public static class UserMapper
{
    public static UserDto MapToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };
    }
}
