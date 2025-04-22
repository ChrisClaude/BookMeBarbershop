using System;
using BookMe.Application.Commands;
using BookMe.Application.Commands.Users;
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
