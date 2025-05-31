using System;
using BookMe.Application.Commands.Users;
using BookMe.Application.Common;
using FluentValidation;

namespace BookMe.Application.Validators;

public class VerifyUserPhoneNumberCommandValidator : AbstractValidator<VerifyUserPhoneNumberCommand>
{
    public VerifyUserPhoneNumberCommandValidator()
    {
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number is required");

        RuleFor(x => x.PhoneNumber)
            .Custom(
                (phoneNumber, context) =>
                {
                    if (!string.IsNullOrWhiteSpace(phoneNumber))
                    {
                        var validationResult = PhoneValidatorHelper.IsValidPhoneNumber(phoneNumber);
                        if (validationResult.IsFailure)
                        {
                            foreach (var error in validationResult.Errors)
                            {
                                context.AddFailure(error.Description);
                            }
                        }
                    }
                }
            );

        RuleFor(x => x.Code).NotEmpty().WithMessage("Verification code is required");
    }
}
