namespace BookMe.Application.Common.Dtos.Users;

public record class UserRoleDto
{
    public UserDto User { get; set; }
    public RoleDto Role { get; set; }
}
