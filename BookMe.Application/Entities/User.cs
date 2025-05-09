using System;
using System.Collections.Generic;

namespace BookMe.Application.Entities;

public class User : BaseEntity
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public List<UserRole> UserRoles { get; set; }
    public List<Booking> Bookings { get; set; }
    public List<TimeSlot> CreatedTimeSlots { get; set; }
    public List<TimeSlot> UpdatedTimeSlots { get; set; }
}
