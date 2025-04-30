
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
            .AddNewtonsoftJson();

        services.AddEndpointsApiExplorer();

        services
            .AddApplication()
            .AddInfrastructure(configuration)
            .ConfigureAuthentication(configuration)
            .ConfigureSerilog(appSettings)
            .ConfigureCors(appSettings, _corsPolicyName)
            .ConfigureOpenApi(appSettings);

        builder.Host.UseSerilog();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddHealthChecks()
            .AddSqlServer(
                configuration.GetConnectionString("BookMeDb"),
                name: "sql-server",
                tags: new[] { "database" })
            .AddElasticsearch(
                appSettings.Elasticsearch.Uri,
                name: "elasticsearch",
                tags: new[] { "logging" })
            .AddCheck<CustomHealthCheck>("custom-check")
            .AddUrlGroup(new Uri($"{appSettings.AzureAdB2C.Instance}/.well-known/openid-configuration"),
                name: "azure-b2c",
                tags: new[] { "auth" });

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

        app.MapHealthChecks("/healthz", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";

                var response = new
                {
                    Status = report.Status.ToString(),
                    Duration = report.TotalDuration,
                    Checks = report.Entries.Select(entry => new
                    {
                        Name = entry.Key,
                        Status = entry.Value.Status.ToString(),
                        Duration = entry.Value.Duration,
                        Description = entry.Value.Description,
                        Exception = entry.Value.Exception?.Message
                    }),
                    Timestamp = DateTime.UtcNow,
                };

                await JsonSerializer.SerializeAsync(context.Response.Body, response);
            }
        });
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
