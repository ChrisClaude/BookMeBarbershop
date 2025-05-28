using BookMe.Application.Commands.Users;
using FluentValidation;

namespace BookMe.Application.Validators;

public class CreateOrUpdateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateOrUpdateUserCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Surname).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Email).EmailAddress();
    }
}
