
using BookMe.Application;
using BookMe.Application.Configurations;
using BookMe.Infrastructure;
using BookMeAPI.Configurations;
using BookMeAPI.MiddleWare;
using Elastic.Transport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Azure;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Serilog;

internal static class WebApplicationConfiguration
{
    private static readonly string _corsPolicyName = "AllowCorsPolicy";

    #region ConfigureServices
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();

        services.Configure<AppSettings>(
            configuration.GetSection("AppSettings"));

        services
            .AddControllers(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                  .RequireAuthenticatedUser()
                  .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            })
            .AddNewtonsoftJson();

        services.AddEndpointsApiExplorer();

        services
            .AddOpenApi((o =>
            {
                o.AddDocumentTransformer((document, _, _) =>
                {
                    document.Components ??= new OpenApiComponents();

                    document.Components.SecuritySchemes.Add("oauth", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            AuthorizationCode = new OpenApiOAuthFlow
                            {
                                // AuthorizationUrl = new Uri($"https://{appSettings.AzureAdB2C.TenantId}.b2clogin.com/{appSettings.AzureAdB2C.Domain}/oauth2/v2.0/authorize?p={appSettings.AzureAdB2C.SignUpSignInPolicyId}"),

                                AuthorizationUrl = new Uri($"https://techvisesandbox.b2clogin.com/techvisesandbox.onmicrosoft.com/{appSettings.AzureAdB2C.SignUpSignInPolicyId}/oauth2/v2.0/authorize"),

                                // TokenUrl = new Uri($"https://{appSettings.AzureAdB2C.Domain}/{appSettings.AzureAdB2C.TenantId}/oauth2/v2.0/token?p={appSettings.AzureAdB2C.SignUpSignInPolicyId}"),
                                TokenUrl = new Uri($"https://techvisesandbox.b2clogin.com/techvisesandbox.onmicrosoft.com/{appSettings.AzureAdB2C.SignUpSignInPolicyId}/oauth2/v2.0/token"),

                                Scopes = new Dictionary<string, string>
                                {
                                    { $"https://{appSettings.AzureAdB2C.TenantId}.onmicrosoft.com/resume-builder-api/Read", "API Read permission" },
                                    { $"https://{appSettings.AzureAdB2C.TenantId}.onmicrosoft.com/resume-builder-api/Write", "API Write permission" },
                                },
                                // To allow Scalar to select PKCE by Default
                                // valid options are 'SHA-256' | 'plain' | 'no'
                                Extensions = new Dictionary<string, IOpenApiExtension>()
                                {
                                    ["x-usePkce"] = new OpenApiString("SHA-256")
                                },

                            }
                        }
                    });

                    return Task.CompletedTask;
                });
            }))
            .AddApplication()
            .AddInfrastructure(configuration)
            .ConfigureAuthentication(configuration)
            .ConfigureSerilog(appSettings)
            .ConfigureCors(appSettings, _corsPolicyName);

        builder.Host.UseSerilog();

        // Add exception handler and problem details
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return builder.Build();
    }
    #endregion


    #region ConfigureRequestPipeline
    public static WebApplication ConfigureRequestPipeline(this WebApplication app)
    {
        var appSettings = app.Configuration.GetSection("AppSettings").Get<AppSettings>();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options
                    .WithPreferredScheme("OAuth2")
                    .WithOAuth2Authentication(oauth2 =>
                    {
                        oauth2.ClientId = appSettings.AzureAdB2C.ClientId;
                        oauth2.Scopes = new[] { $"https://{appSettings.AzureAdB2C.Domain}/resume-builder-api/Read", $"https://{appSettings.AzureAdB2C.Domain}/resume-builder-api/Write" };
                    });

                options.Authentication = new ScalarAuthenticationOptions
                {
                    PreferredSecurityScheme = "OAuth2",
                    OAuth2 = new OAuth2Options
                    {
                        // TokenUrl = $"https://{appSettings.AzureAdB2C.Domain}/{appSettings.AzureAdB2C.TenantId}/oauth2/v2.0/token",
                        ClientId = appSettings.AzureAdB2C.ClientId,
                        // ClientSecret = appSettings.AzureAdB2C.ClientSecret,
                        Scopes = new[] { $"https://{appSettings.AzureAdB2C.Domain}/resume-builder-api/Read", $"https://{appSettings.AzureAdB2C.Domain}/resume-builder-api/Write" },
                    }
                };
            });
        }

        app.UseCors(_corsPolicyName);
        app.UseSerilogRequestLogging();

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseExceptionHandler();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
    #endregion
}
