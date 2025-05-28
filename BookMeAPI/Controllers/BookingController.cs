using BookMe.Application.Commands.Bookings;
using BookMe.Application.Common;
using BookMe.Application.Common.Bookings.Dtos;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Common.Dtos.Bookings;
using BookMe.Application.Interfaces.Queries;
using BookMeAPI.Authorization;
using BookMeAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookMeAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingController(IMediator mediator, ITimeSlotQueries timeSlotQueries) : BaseController
{
    [HttpPost("timeslots/available")]
    [AllowAnonymous]
    [ProducesResponseType<PagedList<TimeSlotDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAvailableTimeSlotsAsync(GetTimeSlotsDto request, [FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10)
    {
        var result = await timeSlotQueries.GetAvailableTimeSlotsAsync(request.Start, request.End, pageIndex, pageSize);

        return result.ToActionResult();
    }

    [HttpPost("timeslots/all")]
    [Authorize(Policy = Policy.ADMIN)]
    [ProducesResponseType<PagedList<TimeSlotDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPagedTimeSlotsAsync(GetTimeSlotsDto request, [FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10)
    {
        var result = await timeSlotQueries.GetPagedTimeSlotsAsync(request.Start, request.End, pageIndex, pageSize);

        return result.ToActionResult();
    }

    [HttpPost]
    [Authorize(Policy = Policy.CUSTOMER)]
    [ProducesResponseType<BookingDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<Result<BookingDto>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> BookTimeSlotsAsync(BookTimeSlotsDto request)
    {
        var result = await mediator.Send(new CreateBookingCommand(request.TimeSlotId));

        return result.ToActionResult();
    }

    [HttpPost]
    [Authorize(Policy = Policy.CUSTOMER)]
    [ProducesResponseType<BookingDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<Result<BookingDto>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CancelBookingAsync(CancelBookingDto request)
    {
        var result = await mediator.Send(new CancelBookingCommand(request.BookingId));

        return result.ToActionResult();
    }

    [HttpPost("timeslots")]
    [Authorize(Policy = Policy.ADMIN)]
    [ProducesResponseType<IEnumerable<TimeSlotDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateTimeSlotsAsync(CreateTimeSlotsDto request)
    {
        var result = await mediator.Send(new CreateTimeSlotCommand(request.StartDateTime, request.EndDateTime, request.IsAllDay));

        return result.ToActionResult();
    }

    [HttpPost("confirm")]
    [Authorize(Policy = Policy.ADMIN)]
    [ProducesResponseType<BookingDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<IEnumerable<Error>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ConfirmBookingAsync(ConfirmBookingDto request)
    {
        var result = await mediator.Send(new ConfirmBookingCommand(request.BookingId));

        return result.ToActionResult();
    }
}

