using BookMe.Application.Commands.Bookings;
using BookMe.Application.Extensions;
using FluentValidation;

namespace BookMe.Application.Validators;

public class CreateBookingCommandValidation : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidation()
    {
        RuleFor(x => x.TimeSlotId).NotEmpty();

        RuleFor(x => x.UserDto).NotNull();
        RuleFor(x => x.UserDto).Must(user => user.IsCustomer())
            .WithMessage(x => $"User {x.UserDto.Id} is not a customer");
    }
}
