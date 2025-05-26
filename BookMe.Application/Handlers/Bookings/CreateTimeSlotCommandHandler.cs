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

public class CreateTimeSlotCommandHandler(IRepository<TimeSlot> timeSlotRepository) : IRequestHandler<CreateTimeSlotCommand, Result<IEnumerable<TimeSlotDto>>>
{
    public async Task<Result<IEnumerable<TimeSlotDto>>> Handle(CreateTimeSlotCommand request, CancellationToken cancellationToken)
    {

        var overlappingSlots = await timeSlotRepository.GetAllAsync(query =>
            query.Where(x =>
                (request.StartDateTime < x.End && request.EndDateTime > x.Start)
            ));

        if (overlappingSlots.Any())
        {
            return Result<IEnumerable<TimeSlotDto>>.Failure(new List<Error> { Error.Conflict("The requested time slot overlaps with existing time slots") }, ErrorType.BadRequest);
        }

        TimeSlot timeSlot;
        var timeSlots = new List<TimeSlotDto>();
        if (request.IsAllDay)
        {
            var count = request.EndDateTime.Date.Subtract(request.StartDateTime.Date).Hours;

            for (var i = 0; i < count; i++)
            {
                timeSlot = new TimeSlot
                {
                    Start = request.StartDateTime.AddHours(i),
                    End = request.StartDateTime.AddHours(i + 1)
                };

                timeSlot.SetAuditableProperties(request.UserDto.Id, AuditEventType.Created);

                await timeSlotRepository.InsertAsync(timeSlot, false);

                timeSlots.Add(timeSlot.MapToDto());
            }

            return Result<IEnumerable<TimeSlotDto>>.Success(timeSlots);
        }

        timeSlot = new TimeSlot
        {
            Start = request.StartDateTime,
            End = request.EndDateTime
        };

        timeSlot.SetAuditableProperties(request.UserDto.Id, AuditEventType.Created);

        await timeSlotRepository.InsertAsync(timeSlot, false);
        timeSlots.Add(timeSlot.MapToDto());


        return Result<IEnumerable<TimeSlotDto>>.Success(timeSlots);
    }
}
