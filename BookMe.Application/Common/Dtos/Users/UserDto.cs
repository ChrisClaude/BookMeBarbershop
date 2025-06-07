using BookMe.Application.Common.Dtos.Users;

namespace BookMe.Application.Common.Dtos;

public record UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool? IsPhoneNumberVerified { get; set; }
    public IEnumerable<UserRoleDto> Roles { get; set; }
}
