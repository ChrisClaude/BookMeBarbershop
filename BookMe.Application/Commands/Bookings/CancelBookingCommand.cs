using System;
using BookMe.Application.Commands.Abstractions;
using BookMe.Application.Common;

namespace BookMe.Application.Commands.Bookings;

public class CancelBookingCommand : AuthenticatedRequest<Result>
{
    public Guid BookingId { get; set; }

    public CancelBookingCommand(Guid bookingId)
    {
        BookingId = bookingId;
    }
}
