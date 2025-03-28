using System;

namespace BookMe.Application.Entities;

public class Booking : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid TimeSlotId { get; set; }
    public BookingStatus Status { get; set; }

    public User User { get; set; }
    public TimeSlot TimeSlot { get; set; }
}
