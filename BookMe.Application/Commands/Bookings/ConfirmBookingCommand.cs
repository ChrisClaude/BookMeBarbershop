using BookMe.Application.Commands.Abstractions;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos.Bookings;

namespace BookMe.Application.Commands.Bookings;

public class ConfirmBookingCommand : AuthenticatedRequest<Result<BookingDto>>
{
    public Guid BookingId { get; set; }

    public ConfirmBookingCommand(Guid bookingId)
    {
        BookingId = bookingId;
    }
}
