using System;
using BookMe.Application.Commands.Bookings;
using FluentValidation;

namespace BookMe.Application.Validators;

public class CreateBookingCommandValidation : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidation()
    {
        RuleFor(x => x.TimeSlotId).NotEmpty();

        RuleFor(x => x.UserDTo).NotNull();
        RuleFor(x => x.UserDTo).Must(user => user.IsCustomer)
            .WithMessage(x => $"User {x.UserDTo.Id} is not a customer");
    }
}
