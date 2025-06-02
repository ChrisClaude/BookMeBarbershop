using BookMe.Application.Entities;

namespace BookMe.Application.Common.Dtos.Bookings;

public record class GetBookingsDto
{
    public DateTime FromDateTime { get; set; }
    public BookingStatus? BookingStatus { get; set; }
}
