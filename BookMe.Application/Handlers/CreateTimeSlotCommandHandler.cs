using BookMe.Application.Commands.Bookings;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Entities;
using BookMe.Application.Extensions;
using BookMe.Application.Interfaces;
using BookMe.Application.Mappings;
using MediatR;
using Serilog;

namespace BookMe.Application.Handlers;

public class CreateTimeCommandHandler(IRepository<TimeSlot> timeSlotRepository) : IRequestHandler<CreateTimeSlotCommand, Result<TimeSlotDto>>
{
    public async Task<Result<TimeSlotDto>> Handle(CreateTimeSlotCommand request, CancellationToken cancellationToken)
    {
        var timeSlot = new TimeSlot
        {
            Start = request.StartDateTime,
            End = request.EndDateTime
        };

        timeSlot.SetAuditableProperties(request.UserDTo.Id, AuditEventType.Created);

        await timeSlotRepository.InsertAsync(timeSlot);

        return Result<TimeSlotDto>.Success(timeSlot.MapToDto());
    }
}
