using BookMe.Application.Common.Dtos;
using BookMe.Application.Common.Dtos.Users;
using BookMe.Application.Entities;
using BookMe.Application.Interfaces;

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
            PhoneNumber = user.PhoneNumber,
            IsPhoneNumberVerified = user.IsPhoneNumberVerified,
            Roles = user.UserRoles?.Select(x => x.MapToDto()),
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
            PhoneNumber = userDto.PhoneNumber,
        };
    }

    public static RoleDto MapToDto(this Role role)
    {
        return new RoleDto { Name = role.Name };
    }

    public static UserRoleDto MapToDto(this UserRole userRole)
    {
        return new UserRoleDto { Role = userRole.Role.MapToDto() };
    }

    public static PagedListDto<UserDto> MapToDto(this IPagedList<User> users)
    {
        return new PagedListDto<UserDto>
        {
            PageIndex = users.PageIndex,
            PageSize = users.PageSize,
            TotalCount = users.TotalCount,
            TotalPages = users.TotalPages,
            HasPreviousPage = users.HasPreviousPage,
            HasNextPage = users.HasNextPage,
            Items = users.Select(x => x.MapToDto()).ToList(),
        };
    }
}
