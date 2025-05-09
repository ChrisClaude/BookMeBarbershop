using System;

namespace BookMe.Application.Common.Bookings.Dtos;

public record CreateTimeSlotsDto
{
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
}
