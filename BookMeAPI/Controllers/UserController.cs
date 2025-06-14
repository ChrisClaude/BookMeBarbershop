using BookMe.Application.Commands.Users;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Common.Dtos.Users;
using BookMe.Application.Interfaces.Queries;
using BookMeAPI.Authorization;
using BookMeAPI.Controllers;
using BookMeAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookMeAPI.Apis;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class UserController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IUserQueries _userQueries;

    public UserController(IMediator mediator, IUserQueries userQueries)
    {
        _mediator = mediator;
        _userQueries = userQueries;
    }

    [HttpGet("me")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<IEnumerable<Error>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserAsync()
    {
        var userDto = GetContextUser();
        var result = await _userQueries.GetUserAsync(userDto.Id);

        return result.ToActionResult();
    }

    [HttpGet("all")]
    [Authorize(Policy = Policy.ADMIN)]
    [ProducesResponseType<PagedListDto<UserDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<IEnumerable<Error>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUsersAsync(
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 10
    )
    {
        var result = await _userQueries.GetUsersAsync(page, pageSize);

        return result.ToActionResult();
    }

    [HttpPut("profile")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<IEnumerable<Error>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateProfileAsync([FromBody] UserUpdateDto request)
    {
        var result = await _mediator.Send(new UpdateUserCommand(request.Name, request.Surname));

        return result.ToActionResult();
    }
}
