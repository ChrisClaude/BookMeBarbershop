
using BookMe.Application;
using BookMe.Application.Configurations;
using BookMe.Infrastructure;
using BookMeAPI.Configurations;
using BookMeAPI.MiddleWare;
using Elastic.Transport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Scalar.AspNetCore;
using Serilog;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Text.Json.Serialization;
using BookMeAPI.HealthChecks;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;

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

        services.AddMemoryCache();

        services
            .AddControllers(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                  .RequireAuthenticatedUser()
                  .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

        services.AddEndpointsApiExplorer();

        services
            .AddApplication()
            .AddInfrastructure(configuration)
            .ConfigureAuthentication(configuration)
            .ConfigureAuthorization()
            .ConfigureSerilog(appSettings)
            .ConfigureOpenTelemetryTracing(appSettings)
            .ConfigureCors(appSettings, _corsPolicyName)
            .ConfigureOpenApi(appSettings);

        builder.Host.UseSerilog();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.ConfigureHealthChecks(appSettings, configuration);

        return builder.Build();
    }
    #endregion


    #region ConfigureRequestPipeline
    public static WebApplication ConfigureRequestPipeline(this WebApplication app)
    {
        var appSettings = app.Configuration.GetSection("AppSettings").Get<AppSettings>();

        if (app.Environment.IsDevelopment())
        {
            app.UseScalar(appSettings);
            app.MigrateDatabase();
        }

        app.MapHealthChecks("/healthz", HealthChecksConfiguration.HealthCheckOptions);
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
