using System;
using BookMe.Application.Commands.Bookings;
using BookMe.Application.Extensions;
using FluentValidation;

namespace BookMe.Application.Validators;

public class CancelBookingCommandValidator : AbstractValidator<CancelBookingCommand>
{
    public CancelBookingCommandValidator()
    {
        RuleFor(x => x.BookingId).NotEmpty();
        RuleFor(x => x)
            .Must(x => x.UserDto.IsCustomer())
            .WithMessage(x => $"User {x.UserDto.Id} is not a customer");
    }
}
