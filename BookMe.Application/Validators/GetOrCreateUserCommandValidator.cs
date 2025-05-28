using BookMe.Application.Commands;
using FluentValidation;

namespace BookMe.Application.Validators;

public class GetOrCreateUserCommandValidator : AbstractValidator<GetOrCreateUserCommand>
{
    public GetOrCreateUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Email).EmailAddress();
    }
}
