namespace BookMe.Application.Common.Dtos.PhoneVerification;

public record class SendCodeRequest
{
    public string PhoneNumber { get; set; }
}
