using BookMe.Application.Common.Dtos;
using BookMe.Application.Enums;
using BookMe.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace BookMeAPI.Controllers;

public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Gets the user from the request context.
    /// </summary>
    /// <returns>The user loaded from the request context.</returns>
    /// <exception cref="HttpContextUserLoadingProcessFailureException">Thrown when the user is not found in the request context.</exception>
    protected UserDto GetContextUser()
    {
        if (HttpContext.Items[Constant.HTTP_CONTEXT_USER_ITEM_KEY] is UserDto userDto)
        {
            return userDto;
        }
        else
        {
            throw new HttpContextUserLoadingProcessFailureException(
                "User details not found in the request context."
            );
        }
    }
}
