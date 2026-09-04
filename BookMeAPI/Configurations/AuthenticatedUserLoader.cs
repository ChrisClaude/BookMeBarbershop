using BookMe.Application.Commands;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Enums;
using BookMe.Application.Exceptions;
using BookMe.Application.Extensions;
using MediatR;

namespace BookMeAPI.Configurations;

/// <summary>
/// Resolves (or creates) the <see cref="UserDto"/> for an authenticated email, shared by the
/// real Azure AD B2C JWT flow (<see cref="AuthenticationConfiguration"/>) and the Testing-only
/// auth flow (<see cref="TestAuthenticationConfiguration"/>) so both populate
/// <c>HttpContext.Items</c> identically.
/// </summary>
public static class AuthenticatedUserLoader
{
    public static async Task<(string key, UserDto user)> LoadAsync(
        IMediator mediator,
        string userEmail
    )
    {
        // The email is validated as part of the command validation
        var result = await mediator.Send(new GetOrCreateUserCommand(userEmail));

        if (result.IsFailure)
        {
            throw new HttpContextUserLoadingProcessFailureException(
                result.Errors.ToAggregateString()
            );
        }

        return (Constant.HTTP_CONTEXT_USER_ITEM_KEY, result.Value);
    }
}