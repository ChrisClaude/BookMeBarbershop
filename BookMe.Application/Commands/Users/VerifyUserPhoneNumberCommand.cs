using BookMe.Application.Commands.Abstractions;
using BookMe.Application.Common;

namespace BookMe.Application.Commands.Users;

public class UpdateUserPhoneNumberCommand : AuthenticatedRequest<Result>
{
    public string PhoneNumber { get; set; }

    public string Code { get; set; }

    public UpdateUserPhoneNumberCommand(string phoneNumber, string code)
    {
        PhoneNumber = phoneNumber;
        Code = code;
    }
}
