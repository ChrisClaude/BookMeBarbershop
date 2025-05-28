using BookMe.Application.Commands.Bookings;
using BookMe.Application.Extensions;
using FluentValidation;

namespace BookMe.Application.Validators;

public class ConfirmBookingCommandValidator : AbstractValidator<ConfirmBookingCommand>
{
    public ConfirmBookingCommandValidator()
    {
        RuleFor(x => x.BookingId).NotEmpty();
        RuleFor(x => x.UserDto).Must(x => x.IsAdmin())
            .WithMessage(x => $"User {x.UserDto.Id} is not an admin");
    }
}
