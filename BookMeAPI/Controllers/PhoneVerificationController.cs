using BookMe.Application.Common.Dtos.PhoneVerification;
using BookMe.Application.Interfaces;
using BookMeAPI.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace BookMeAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PhoneVerificationController : BaseController
{
    private readonly ITwilioSmsService _twilioSmsService;

    public PhoneVerificationController(ITwilioSmsService twilioSmsService)
    {
        _twilioSmsService = twilioSmsService;
    }

    [HttpPost("send-code")]
    public async Task<IActionResult> SendCodeAsync([FromBody] SendCodeRequest request)
    {
        var result = await _twilioSmsService.SendVerificationCodeAsync(request.PhoneNumber);

        return result.ToActionResult();
    }

    [HttpPost("verify-code")]
    public async Task<IActionResult> VerifyCodeAsync([FromBody] VerifyCodeRequest request)
    {
        var result = await _twilioSmsService.VerifyCodeAsync(request.PhoneNumber, request.Code);

        return result.ToActionResult();
    }
}
