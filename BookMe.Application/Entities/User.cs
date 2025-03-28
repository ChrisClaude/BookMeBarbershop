using System;

namespace BookMe.Application.Entities;

public class User : BaseEntity
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public List<UserRole> UserRoles { get; set; }
    public List<Booking> Bookings { get; set; }
}
