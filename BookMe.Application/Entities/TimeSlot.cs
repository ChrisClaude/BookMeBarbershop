using System;

namespace BookMe.Application.Entities;

public class TimeSlot : BaseEntity, IAuditable
{
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public Booking Booking { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public User CreatedByUser { get; set; }
    public User UpdatedByUser { get; set; }
}
