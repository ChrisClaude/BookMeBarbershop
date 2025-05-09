using System;
using BookMe.Application.Common.Dtos.Bookings;
using BookMe.Application.Entities;

namespace BookMe.Application.Mappings;

public static class BookingMapper
{
    public static BookingDto MapToDto(this Booking booking)
    {
        return new BookingDto
        {
            Id = booking.Id,
            Status = booking.Status,
            User = booking.User.MapToDto(),
            TimeSlot = booking.TimeSlot.MapToDto()
        };
    }
}
