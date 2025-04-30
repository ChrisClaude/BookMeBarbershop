using System;
using System.Text.Json;
using BookMe.Application.Configurations;
using BookMeAPI.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace BookMeAPI.Configurations;

public static class HealthChecksConfiguration
{
    public static HealthCheckOptions HealthCheckOptions = new()
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
    };
    public static IServiceCollection ConfigureHealthChecks(this IServiceCollection services, AppSettings appSettings, IConfiguration configuration)
    {
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
        return services;
    }
}
