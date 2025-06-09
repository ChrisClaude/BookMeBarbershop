using BookMe.Application.Commands.Bookings;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos.Bookings;
using BookMe.Application.Common.Errors;
using BookMe.Application.Entities;
using BookMe.Application.Interfaces;
using BookMe.Application.Mappings;
using MediatR;

namespace BookMe.Application.Handlers.Bookings;

public class ConfirmBookingCommandHandler(IRepository<Booking> bookingRepository)
    : IRequestHandler<ConfirmBookingCommand, Result<BookingDto>>
{
    public async Task<Result<BookingDto>> Handle(
        ConfirmBookingCommand request,
        CancellationToken cancellationToken
    )
    {
        var booking = await bookingRepository.GetByIdAsync(request.BookingId);
        var errors = new List<Error>();

        if (booking == null)
        {
            errors.Add(Error.NotFound($"Booking with id {request.BookingId} not found"));
        }

        if (booking != null && booking.Status != BookingStatus.Pending)
        {
            errors.Add(Error.InvalidArgument($"Booking {request.BookingId} is not pending"));
        }

        if (errors.Any())
        {
            return Result<BookingDto>.Failure(errors, ErrorType.BadRequest);
        }

        booking.Status = BookingStatus.Confirmed;
        await bookingRepository.UpdateAsync(booking, false);

        return Result<BookingDto>.Success(booking.MapToDto());
    }
}
