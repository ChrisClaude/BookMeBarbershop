using System;
using BookMe.Application.Commands.Bookings;
using FluentValidation;

namespace BookMe.Application.Validators;

public class ConfirmBookingCommandValidator : AbstractValidator<ConfirmBookingCommand>
{
    public ConfirmBookingCommandValidator()
    {
        RuleFor(x => x.BookingId).NotEmpty();
        RuleFor(x => x).Must(x => x.UserDTo.IsAdmin)
            .WithMessage(x => $"User {x.UserDTo.Id} is not an admin");
    }
}
