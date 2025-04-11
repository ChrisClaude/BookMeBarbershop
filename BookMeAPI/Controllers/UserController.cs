using BookMe.Application.Common.Dtos;
using BookMe.Application.Interfaces.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BookMeAPI.Apis;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class UserController : ControllerBase
{
    private readonly IUserQueries _userQueries;

    public UserController(IUserQueries userQueries)
    {
        _userQueries = userQueries;
    }

    [HttpGet("me")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserAsync(Guid userId)
    {
        var result = await _userQueries.GetUserAsync(userId);

        return Ok("test user");
    }
}
