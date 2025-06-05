using BookMe.Application.Commands.Bookings;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos.Bookings;
using BookMe.Application.Common.Errors;
using BookMe.Application.Entities;
using BookMe.Application.Interfaces;
using BookMe.Application.Mappings;
using MediatR;
using Serilog;

namespace BookMe.Application.Handlers;

public class CreateBookingCommandHandler(
    IRepository<Booking> repository,
    IRepository<TimeSlot> timeSlotRepository
) : IRequestHandler<CreateBookingCommand, Result<BookingDto>>
{
    public async Task<Result<BookingDto>> Handle(
        CreateBookingCommand request,
        CancellationToken cancellationToken
    )
    {
        var timeSlot = await timeSlotRepository.GetByIdAsync(
            request.TimeSlotId,
            new string[] { nameof(TimeSlot.Bookings) }
        );
        var errors = new List<Error>();
        if (timeSlot == null)
        {
            Log.Warning(
                "Time slot with id {TimeSlotId} not found by user {UserId}",
                request.TimeSlotId,
                request.UserDto.Id
            );
            errors.Add(
                Error.NotFound(
                    $"Time slot with id {request.TimeSlotId} not found by user {request.UserDto.Id}"
                )
            );
        }

        if (timeSlot != null && !timeSlot.IsAvailable)
        {
            errors.Add(Error.NotFound($"Time slot with id {request.TimeSlotId} is not available"));
        }

        if (errors.Any())
        {
            return Result<BookingDto>.Failure(errors, ErrorType.BadRequest);
        }

        var booking = new Booking
        {
            UserId = request.UserDto.Id,
            TimeSlotId = request.TimeSlotId,
            Status = BookingStatus.Pending,
        };

        await repository.InsertAsync(booking, false);

        return Result<BookingDto>.Success(booking.MapToDto());
    }
}
