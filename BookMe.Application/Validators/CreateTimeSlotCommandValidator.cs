using BookMe.Application.Commands.Bookings;
using FluentValidation;

namespace BookMe.Application.Validators;

public class CreateTimeSlotCommandValidator : AbstractValidator<CreateTimeSlotCommand>
{
    public CreateTimeSlotCommandValidator()
    {
        RuleFor(x => x.StartDateTime).NotEmpty();
        RuleFor(x => x.EndDateTime).NotEmpty();
        RuleFor(x => x).Must(x => x.StartDateTime < x.EndDateTime)
            .WithMessage(x => $"Start time {x.StartDateTime} must be before end time {x.EndDateTime}");
        RuleFor(x => x.UserDTo).Must(x => x.IsAdmin)
            .WithMessage(x => $"User {x.UserDTo.Id} is not an admin");
    }
}