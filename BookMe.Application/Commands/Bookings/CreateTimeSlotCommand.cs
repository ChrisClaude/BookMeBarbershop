using BookMe.Application.Commands.Abstractions;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;

namespace BookMe.Application.Commands.Bookings;

public class CreateTimeSlotCommand : AuthenticatedRequest<Result<TimeSlotDto>>
{
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public CreateTimeSlotCommand(DateTime startDateTime, DateTime endDateTime)
    {
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
    }
}
