using System;
using BookMe.Application.Commands.Users;
using BookMe.Application.Entities;
using FluentValidation;

namespace BookMe.Application.Validators;

public class CreateOrUpdateUserCommandValidator : AbstractValidator<CreateOrUpdateUserCommand>
{
    public CreateOrUpdateUserCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Surname).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
    }
}
