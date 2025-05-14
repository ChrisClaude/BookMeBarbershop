namespace BookMe.Application.Common.Dtos.Bookings;

public record class CancelBookingDto
{
    public Guid BookingId { get; set; }
}
