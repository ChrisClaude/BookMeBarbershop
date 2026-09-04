using BookMeAPI.Authentication;
using Microsoft.AspNetCore.Authentication;

namespace BookMeAPI.Configurations;

/// <summary>
/// Registers header-based authentication for ASPNETCORE_ENVIRONMENT=Testing, used by
/// BookMe.IntegrationTests in place of real Azure AD B2C JWT auth. See
/// <see cref="TestAuthHandler"/>.
/// </summary>
public static class TestAuthenticationConfiguration
{
    public static IServiceCollection ConfigureTestAuthentication(this IServiceCollection services)
    {
        services
            .AddAuthentication(TestAuthHandler.SCHEME_NAME)
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                TestAuthHandler.SCHEME_NAME,
                _ => { }
            );

        return services;
    }
}