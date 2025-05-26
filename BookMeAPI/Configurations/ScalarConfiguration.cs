using BookMe.Application.Configurations;
using Scalar.AspNetCore;

namespace BookMeAPI.Configurations;

public static class ScalarConfiguration
{
    public static void UseScalar(this WebApplication app, AppSettings appSettings)
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options
                .WithPreferredScheme("OAuth2")
                .WithOAuth2Authentication(oauth2 =>
                {
                    oauth2.ClientId = appSettings.AzureAdB2C.ClientId;
                    oauth2.Scopes = new[]
                    {
                        $"https://{appSettings.AzureAdB2C.Domain}/resume-builder-api/Read",
                        $"https://{appSettings.AzureAdB2C.Domain}/resume-builder-api/Write"
                    };
                });

            options.Authentication = new ScalarAuthenticationOptions
            {
                PreferredSecurityScheme = "OAuth2",
                OAuth2 = new OAuth2Options
                {
                    ClientId = appSettings.AzureAdB2C.ClientId,
                    Scopes = new[]
                    {
                        $"https://{appSettings.AzureAdB2C.Domain}/resume-builder-api/Read",
                        $"https://{appSettings.AzureAdB2C.Domain}/resume-builder-api/Write"
                    },
                }
            };
        });
    }
}
