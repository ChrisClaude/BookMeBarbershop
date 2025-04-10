
using BookMe.Application;
using BookMe.Application.Configurations;
using BookMe.Infrastructure;
using BookMeAPI.Configurations;
using BookMeAPI.MiddleWare;
using Elastic.Transport;
using Microsoft.Extensions.Azure;
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
            .AddOpenApi()
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
                        oauth2.TokenUrl = $"https://{appSettings.AzureAdB2C.Domain}/{appSettings.AzureAdB2C.TenantId}/oauth2/v2.0/token";
                        oauth2.ClientId = appSettings.AzureAdB2C.ClientId;
                        oauth2.ClientSecret = appSettings.AzureAdB2C.ClientSecret;
                        oauth2.Scopes = new[] { $"https://{appSettings.AzureAdB2C.Domain}/resume-builder-api/Read", $"https://{appSettings.AzureAdB2C.Domain}/resume-builder-api/Write" };
                    });

                options.Authentication = new ScalarAuthenticationOptions
                {
                    PreferredSecurityScheme = "OAuth2",
                    OAuth2 = new OAuth2Options
                    {
                        TokenUrl = $"https://{appSettings.AzureAdB2C.Domain}/{appSettings.AzureAdB2C.TenantId}/oauth2/v2.0/token",
                        ClientId = appSettings.AzureAdB2C.ClientId,
                        ClientSecret = appSettings.AzureAdB2C.ClientSecret,
                        Scopes = new[] { $"https://{appSettings.AzureAdB2C.Domain}/resume-builder-api/Read", $"https://{appSettings.AzureAdB2C.Domain}/resume-builder-api/Write" }
                    }
                };
            });
        }

        app.UseCors(_corsPolicyName);
        app.UseSerilogRequestLogging();

        app.UseHttpsRedirection();
        app.UseExceptionHandler();

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
    #endregion
}
