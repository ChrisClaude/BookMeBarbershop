using BookMe.Application.Commands.Abstractions;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;
using MediatR;

namespace BookMe.Application.Commands.Users;

public class VerifyUserPhoneNumberCommand : AuthenticatedRequest<Result>
{
    public string PhoneNumber { get; set; }

    public string Code { get; set; }

    public VerifyUserPhoneNumberCommand(string phoneNumber, string code)
    {
        PhoneNumber = phoneNumber;
        Code = code;
    }
}
