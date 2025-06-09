namespace BookMe.Application.Common.Dtos.Users;

public record class UserUpdateDto
{
    public string Name { get; set; }
    public string Surname { get; set; }
}
