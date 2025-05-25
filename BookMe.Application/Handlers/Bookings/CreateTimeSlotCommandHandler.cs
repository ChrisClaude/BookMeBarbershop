using BookMe.Application.Commands.Bookings;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Common.Errors;
using BookMe.Application.Entities;
using BookMe.Application.Extensions;
using BookMe.Application.Interfaces;
using BookMe.Application.Mappings;
using MediatR;

namespace BookMe.Application.Handlers;

public class CreateTimeCommandHandler(IRepository<TimeSlot> timeSlotRepository) : IRequestHandler<CreateTimeSlotCommand, Result<TimeSlotDto>>
{
    public async Task<Result<TimeSlotDto>> Handle(CreateTimeSlotCommand request, CancellationToken cancellationToken)
    {

        var overlappingSlots = await timeSlotRepository.GetAllAsync(query =>
            query.Where(x =>
                (request.StartDateTime < x.End && request.EndDateTime > x.Start)
            ));

        if (overlappingSlots.Any())
        {
            return Result<TimeSlotDto>.Failure(new List<Error> { Error.Conflict("The requested time slot overlaps with existing time slots") }, ErrorType.BadRequest);
        }

        var timeSlot = new TimeSlot
        {
            Start = request.StartDateTime,
            End = request.EndDateTime
        };

        timeSlot.SetAuditableProperties(request.UserDto.Id, AuditEventType.Created);

        await timeSlotRepository.InsertAsync(timeSlot, false);

        return Result<TimeSlotDto>.Success(timeSlot.MapToDto());
    }
}
