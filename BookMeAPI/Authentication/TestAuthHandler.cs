using System.Security.Claims;
using System.Text.Encodings.Web;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Exceptions;
using BookMeAPI.Configurations;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace BookMeAPI.Authentication;

/// <summary>
/// Authenticates requests using the <c>X-Test-User-Email</c> header instead of a real Azure AD
/// B2C JWT. Registered only under ASPNETCORE_ENVIRONMENT=Testing (see
/// <see cref="TestAuthenticationConfiguration"/>), so BookMe.IntegrationTests can drive the API
/// as a specific seeded user (admin/customer/etc.) without a live B2C token, while
/// Development/Production keep real JWT auth untouched.
/// </summary>
public class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IMediator mediator
) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SCHEME_NAME = "Test";
    public const string USER_EMAIL_HEADER = "X-Test-User-Email";

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(USER_EMAIL_HEADER, out var userEmail))
        {
            return AuthenticateResult.Fail($"Missing {USER_EMAIL_HEADER} header.");
        }

        (string key, UserDto user) loaded;
        try
        {
            loaded = await AuthenticatedUserLoader.LoadAsync(mediator, userEmail!);
        }
        catch (HttpContextUserLoadingProcessFailureException ex)
        {
            return AuthenticateResult.Fail(ex);
        }

        Context.Items.Add(new KeyValuePair<object, object>(loaded.key, loaded.user));

        var claims = new[] { new Claim("emails", userEmail!) };
        var identity = new ClaimsIdentity(claims, SCHEME_NAME);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SCHEME_NAME);

        return AuthenticateResult.Success(ticket);
    }
}