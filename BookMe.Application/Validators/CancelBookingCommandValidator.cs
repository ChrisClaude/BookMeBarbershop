using System;
using BookMe.Application.Commands.Bookings;
using FluentValidation;

namespace BookMe.Application.Validators;

public class CancelBookingCommandValidator : AbstractValidator<CancelBookingCommand>
{
    public CancelBookingCommandValidator()
    {
        RuleFor(x => x.BookingId).NotEmpty();
        RuleFor(x => x).Must(x => x.UserDTo.IsCustomer)
            .WithMessage(x => $"User {x.UserDTo.Id} is not a customer");
    }
}
