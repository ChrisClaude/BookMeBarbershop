using BookMe.Application.Common.Dtos;
using BookMe.Application.Enums;
using BookMe.Application.Interfaces.Queries;
using BookMeAPI.Controllers;
using BookMeAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BookMeAPI.Apis;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class UserController : BaseController
{
    private readonly IUserQueries _userQueries;

    public UserController(IMediator mediator, IUserQueries userQueries)
    {
        _userQueries = userQueries;
    }

    [HttpGet("me")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserAsync()
    {
        var userDto = GetContextUser();
        var result = await _userQueries.GetUserAsync(userDto.Id);

        return result.ToActionResult();
    }
}
