using System;

namespace BookMe.Application.Common.Bookings.Dtos;

public record BookTimeSlotsDto
{
    public Guid TimeSlotId { get; set; }
}
