using BookMe.Application.Commands.Abstractions;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;

namespace BookMe.Application.Commands.Bookings;

public class CreateTimeSlotCommand : AuthenticatedRequest<Result<IEnumerable<TimeSlotDto>>>
{
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public bool IsAllDay { get; }
    public bool AllowAutoConfirmation { get; }

    public CreateTimeSlotCommand(
        DateTime startDateTime,
        DateTime endDateTime,
        bool isAllDay = false,
        bool allowAutoConfirmation = false
    )
    {
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        IsAllDay = isAllDay;
        AllowAutoConfirmation = allowAutoConfirmation;
    }
}
