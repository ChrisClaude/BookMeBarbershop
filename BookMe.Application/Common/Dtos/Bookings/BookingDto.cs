using BookMe.Application.Entities;

namespace BookMe.Application.Common.Dtos.Bookings;

public record class BookingDto
{
    public Guid Id { get; set; }
    public BookingStatus Status { get; set; }

    public UserDto User { get; set; }
    public TimeSlotDto TimeSlot { get; set; }
}
