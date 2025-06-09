namespace BookMe.Application.Common.Dtos.PhoneVerification;

public record class VerifyCodeRequest
{
    public string PhoneNumber { get; set; }
    public string Code { get; set; }
}
