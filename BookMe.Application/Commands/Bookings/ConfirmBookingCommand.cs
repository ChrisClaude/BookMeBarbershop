using System;
using BookMe.Application.Commands.Abstractions;
using BookMe.Application.Common;

namespace BookMe.Application.Commands.Bookings;

public class ConfirmBookingCommand : AuthenticatedRequest<Result>
{
    public Guid BookingId { get; set; }

    public ConfirmBookingCommand(Guid bookingId)
    {
        BookingId = bookingId;
    }
}
