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

public class CreateTimeSlotCommandHandler(IRepository<TimeSlot> timeSlotRepository)
    : IRequestHandler<CreateTimeSlotCommand, Result<IEnumerable<TimeSlotDto>>>
{
    public async Task<Result<IEnumerable<TimeSlotDto>>> Handle(
        CreateTimeSlotCommand request,
        CancellationToken cancellationToken
    )
    {
        var overlappingSlots = await timeSlotRepository.GetAllAsync(query =>
            query.Where(x => (request.StartDateTime < x.End && request.EndDateTime > x.Start))
        );

        if (overlappingSlots.Any())
        {
            return Result<IEnumerable<TimeSlotDto>>.Failure(
                new List<Error>
                {
                    Error.Conflict("The requested time slot overlaps with existing time slots"),
                },
                ErrorType.BadRequest
            );
        }

        if (request.IsAllDay)
        {
            var count = request.EndDateTime.Subtract(request.StartDateTime).Hours;

            var newTimeSlotList = GenerateTimeSlotsForAllDay(request, count);

            await timeSlotRepository.InsertAsync(newTimeSlotList, false);

            return Result<IEnumerable<TimeSlotDto>>.Success(
                newTimeSlotList.Select(x => x.MapToDto()).ToList()
            );
        }

        var timeSlot = new TimeSlot
        {
            Start = request.StartDateTime,
            End = request.EndDateTime,
            AllowAutoConfirmation = request.AllowAutoConfirmation,
        };

        timeSlot.SetAuditableProperties(request.UserDto.Id, AuditEventType.Created);

        await timeSlotRepository.InsertAsync(timeSlot, false);

        return Result<IEnumerable<TimeSlotDto>>.Success(
            new List<TimeSlotDto> { timeSlot.MapToDto() }
        );
    }

    private static List<TimeSlot> GenerateTimeSlotsForAllDay(
        CreateTimeSlotCommand request,
        int count
    )
    {
        var newTimeSlotList = new List<TimeSlot>();

        for (var i = 0; i < count; i++)
        {
            var timeSlot = new TimeSlot
            {
                Start = request.StartDateTime.AddHours(i),
                End = request.StartDateTime.AddHours(i + 1),
                AllowAutoConfirmation = request.AllowAutoConfirmation,
            };

            timeSlot.SetAuditableProperties(request.UserDto.Id, AuditEventType.Created);

            newTimeSlotList.Add(timeSlot);
        }

        return newTimeSlotList;
    }
}
