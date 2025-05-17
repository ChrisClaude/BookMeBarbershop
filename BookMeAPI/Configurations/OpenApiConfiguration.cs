using System;
using BookMe.Application.Configurations;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace BookMeAPI.Configurations;

public static class OpenApiConfiguration
{
    public static IServiceCollection ConfigureOpenApi(
        this IServiceCollection services,
        AppSettings appSettings
    )
    {
        services.AddOpenApi(
            (
                o =>
                {
                    o.AddDocumentTransformer(
                        (document, _, _) =>
                        {
                            document.Components ??= new OpenApiComponents();

                            document.Components.SecuritySchemes.Add(
                                "oauth",
                                new OpenApiSecurityScheme
                                {
                                    Type = SecuritySchemeType.OAuth2,
                                    Flows = new OpenApiOAuthFlows
                                    {
                                        AuthorizationCode = new OpenApiOAuthFlow
                                        {
                                            AuthorizationUrl = new Uri(
                                                $"{appSettings.AzureAdB2C.Instance}/{appSettings.AzureAdB2C.Domain}/{appSettings.AzureAdB2C.SignUpSignInPolicyId}/oauth2/v2.0/authorize"
                                            ),
                                            TokenUrl = new Uri(
                                                $"{appSettings.AzureAdB2C.Instance}/{appSettings.AzureAdB2C.Domain}/{appSettings.AzureAdB2C.SignUpSignInPolicyId}/oauth2/v2.0/token"
                                            ),
                                            Scopes = new Dictionary<string, string>
                                            {
                                                {
                                                    $"https://{appSettings.AzureAdB2C.TenantId}.onmicrosoft.com/resume-builder-api/Read",
                                                    "API Read permission"
                                                },
                                                {
                                                    $"https://{appSettings.AzureAdB2C.TenantId}.onmicrosoft.com/resume-builder-api/Write",
                                                    "API Write permission"
                                                },
                                            },
                                            // To allow Scalar to select PKCE by Default
                                            // valid options are 'SHA-256' | 'plain' | 'no'
                                            Extensions = new Dictionary<string, IOpenApiExtension>()
                                            {
                                                ["x-usePkce"] = new OpenApiString("SHA-256")
                                            },
                                        }
                                    }
                                }
                            );

                            return Task.CompletedTask;
                        }
                    );
                }
            )
        );
        return services;
    }
}
