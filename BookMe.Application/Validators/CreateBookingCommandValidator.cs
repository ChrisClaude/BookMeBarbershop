using System;
using BookMe.Application.Commands.Bookings;
using FluentValidation;

namespace BookMe.Application.Validators;

public class CreateBookingCommandValidation : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidation()
    {
        RuleFor(x => x.TimeSlotId).NotEmpty();
    }
}
