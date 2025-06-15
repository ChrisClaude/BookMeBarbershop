using System.Text.Json;
using BookMe.Application.Configurations;
using BookMeAPI.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

namespace BookMeAPI.Configurations;

public static class HealthChecksConfiguration
{
    public static readonly HealthCheckOptions HealthCheckOptions = new()
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
                    Exception = entry.Value.Exception?.Message,
                }),
                Timestamp = DateTime.UtcNow,
            };

            await JsonSerializer.SerializeAsync(context.Response.Body, response);
        },
    };

    public static IServiceCollection ConfigureHealthChecks(
        this IServiceCollection services,
        AppSettings appSettings,
        IConfiguration configuration
    )
    {
        var healthChecks = services
            .AddHealthChecks()
            .AddSqlServer(
                configuration.GetConnectionString("BookMeDb"),
                name: "sql-server",
                tags: new[] { "database" }
            )
            // TODO: Uncomment when this issue is resolved https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/issues/2355
            // .AddElasticsearch(
            //     appSettings.Elasticsearch.Uri,
            //     name: "elasticsearch",
            //     tags: new[] { "logging" })
            .AddCheck<CustomHealthCheck>("custom-check");

        if (
            appSettings != null
            && appSettings.AzureAdB2C != null
            && !string.IsNullOrEmpty(appSettings.AzureAdB2C.Instance)
            && !string.IsNullOrEmpty(appSettings.AzureAdB2C.Domain)
            && !string.IsNullOrEmpty(appSettings.AzureAdB2C.SignUpSignInPolicyId)
        )
        {
            try
            {
                var uriString =
                    $"{appSettings.AzureAdB2C.Instance}/{appSettings.AzureAdB2C.Domain}/{appSettings.AzureAdB2C.SignUpSignInPolicyId}/v2.0/.well-known/openid-configuration";
                var uri = new Uri(uriString);

                healthChecks.AddUrlGroup(uri, name: "azure-b2c", tags: new[] { "auth" });
            }
            catch (UriFormatException ex)
            {
                Log.Error($"Invalid Azure B2C URI configuration: {ex.Message}");
            }
        }

        return services;
    }
}
