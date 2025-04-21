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

    public static User MapToEntity(this UserDto userDto)
    {
        return new User
        {
            Id = userDto.Id,
            Name = userDto.Name,
            Surname = userDto.Surname,
            Email = userDto.Email,
            PhoneNumber = userDto.PhoneNumber
        };
    }
}
