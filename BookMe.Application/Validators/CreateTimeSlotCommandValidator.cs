using System.Data;
using BookMe.Application.Commands.Bookings;
using BookMe.Application.Extensions;
using FluentValidation;

namespace BookMe.Application.Validators;

public class CreateTimeSlotCommandValidator : AbstractValidator<CreateTimeSlotCommand>
{
    public CreateTimeSlotCommandValidator()
    {
        RuleFor(x => x.StartDateTime).NotEmpty();
        RuleFor(x => x.EndDateTime).NotEmpty();
        RuleFor(x => x)
            .Must(x => x.StartDateTime < x.EndDateTime)
            .WithMessage(
                x => $"Start time {x.StartDateTime} must be before end time {x.EndDateTime}"
            );

        RuleFor(x => x)
            .Must(
                x =>
                    !x.IsAllDay
                    && x.EndDateTime - x.StartDateTime
                        <= TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(1))
            )
            .WithMessage(x => $"Time slot must not be apart by more than 1 hour");

        RuleFor(x => x)
            .Must(
                x =>
                    !x.IsAllDay
                    || (
                        x.IsAllDay
                        && x.EndDateTime - x.StartDateTime
                            <= TimeSpan.FromHours(24).Add(TimeSpan.FromMinutes(1))
                    )
            )
            .WithMessage(x => $"All-day time slots must not be apart by more than 24 hours");

        RuleFor(x => x)
            .Must(x => x.StartDateTime > DateTime.UtcNow)
            .WithMessage(x => $"Start time {x.StartDateTime} must be in the future");

        RuleFor(x => x.UserDto)
            .Must(x => x.IsAdmin())
            .WithMessage(x => $"User {x.UserDto.Id} is not an admin");
    }
}
