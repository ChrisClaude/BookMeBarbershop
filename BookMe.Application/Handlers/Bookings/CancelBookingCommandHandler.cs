using BookMe.Application.Commands.Bookings;
using BookMe.Application.Common;
using BookMe.Application.Common.Errors;
using BookMe.Application.Entities;
using BookMe.Application.Interfaces;
using MediatR;

namespace BookMe.Application.Handlers;

public class CancelBookingCommandHandler(IRepository<Booking> bookingRepository)
    : IRequestHandler<CancelBookingCommand, Result>
{
    public async Task<Result> Handle(
        CancelBookingCommand request,
        CancellationToken cancellationToken
    )
    {
        var booking = await bookingRepository.GetByIdAsync(
            request.BookingId,
            new[] { nameof(Booking.User) }
        );

        var errors = new List<Error>();
        if (booking == null)
        {
            errors.Add(Error.NotFound($"Booking with id {request.BookingId} not found"));
        }

        if (booking != null)
        {
            if (booking.User.Id != request.UserDto.Id)
                errors.Add(
                    Error.NotAuthorized(
                        $"User {request.UserDto.Id} is not authorized to cancel booking {request.BookingId}"
                    )
                );

            if (booking.Status == BookingStatus.Cancelled)
                errors.Add(
                    Error.InvalidArgument($"Booking {request.BookingId} is already cancelled")
                );
        }

        if (errors.Any())
        {
            return Result.Failure(errors, ErrorType.BadRequest);
        }

        booking.Status = BookingStatus.Cancelled;

        // TODO: an event should be published here
        await bookingRepository.UpdateAsync(booking, false);

        return Result.Success(ResultSuccessType.Updated);
    }
}
