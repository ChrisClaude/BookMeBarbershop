using System;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Entities;

namespace BookMe.Application.Mappings;

public static class TimeSlotMapper
{
    public static TimeSlotDto MapToDto(this TimeSlot timeSlot)
    {
        return new TimeSlotDto
        {
            Id = timeSlot.Id,
            Start = timeSlot.Start,
            End = timeSlot.End,
            IsAvailable = timeSlot.Booking == null
        };
    }
}
