using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BookMeAPI.Apis;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class UserController : ControllerBase
{

    [HttpGet("me")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserAsync()
    {
        return Ok("test user");
    }
}
