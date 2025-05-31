using BookMe.Application.Common;

namespace BookMe.Application.Interfaces;

public interface IUserService
{
    Task<Result> VerifyPhoneNumberAsync(string phoneNumber, string code);
}
