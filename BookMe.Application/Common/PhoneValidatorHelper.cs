using System;
using PhoneNumbers;

namespace BookMe.Application.Common;

public class PhoneValidatorHelper
{
    public static Result IsValidPhoneNumber(string phoneNumber)
    {
        var phoneUtil = PhoneNumberUtil.GetInstance();

        PhoneNumber numberProto;
        try
        {
            numberProto = phoneUtil.Parse(phoneNumber, null); // null for default region because the phoneNumber is expected to be in E.164 format
        }
        catch (NumberParseException e)
        {
            return Result.Failure(
                new Error("phone-number-format", $"Invalid phone number format: {e.Message}"),
                Errors.ErrorType.None
            );
        }

        if (!phoneUtil.IsValidNumber(numberProto))
        {
            return Result.Failure(
                new Error(
                    "invalid-phone-number",
                    "The provided phone number is not a valid number."
                ),
                Errors.ErrorType.None
            );
        }

        return Result.Success();
    }
}
