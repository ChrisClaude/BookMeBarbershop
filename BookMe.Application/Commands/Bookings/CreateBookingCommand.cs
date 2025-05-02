using BookMe.Application.Commands.Abstractions;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos.Bookings;

namespace BookMe.Application.Commands.Bookings;

public class CreateBookingCommand : AuthenticatedRequest<Result<BookingDto>>
{

    public CreateBookingCommand(Guid timeSlotId)
    {
        TimeSlotId = timeSlotId;
    }

    public Guid TimeSlotId { get; }
}
