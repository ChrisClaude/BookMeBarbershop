using BookMe.Application.Commands.Users;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos.Bookings;
using BookMe.Application.Common.Dtos.PhoneVerification;
using BookMe.Application.Interfaces;
using BookMeAPI.Authorization;
using BookMeAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookMeAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PhoneVerificationController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ITwilioSmsService _twilioSmsService;

    public PhoneVerificationController(IMediator mediator, ITwilioSmsService twilioSmsService)
    {
        _mediator = mediator;
        _twilioSmsService = twilioSmsService;
    }

    [HttpPost("send-code")]
    [Authorize(Policy = Policy.CUSTOMER)]
    [ProducesResponseType<Result>(StatusCodes.Status200OK)]
    [ProducesResponseType<IEnumerable<Error>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SendCodeAsync([FromBody] SendCodeRequest request)
    {
        var result = await _twilioSmsService.SendVerificationCodeAsync(request.PhoneNumber);

        return result.ToActionResult();
    }

    [HttpPost("verify-code")]
    [Authorize(Policy = Policy.CUSTOMER)]
    [ProducesResponseType<Result>(StatusCodes.Status200OK)]
    [ProducesResponseType<IEnumerable<Error>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> VerifyCodeAsync([FromBody] VerifyCodeRequest request)
    {
        var result = await _mediator.Send(
            new VerifyUserPhoneNumberCommand(request.PhoneNumber, request.Code)
        );

        return result.ToActionResult();
    }
}
