using Azure.Monitor.OpenTelemetry.AspNetCore;
using BookMe.Application.Configurations;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BookMeAPI.Configurations;

public static class OpenTelemetryTracingConfiguration
{
    public static IServiceCollection ConfigureOpenTelemetryTracing(
        this IServiceCollection services,
        AppSettings appSettings
    )
    {
        var builder = services.AddOpenTelemetry();

        builder
            .ConfigureResource(resource => resource.AddService("BookMeAPI"))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation();

                tracing.AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(appSettings.OpenTelemetry.Seq.TracesUri);
                    otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                    otlpOptions.Headers = $"X-Seq-ApiKey={appSettings.OpenTelemetry.Seq.ApiKey}";
                });
            });

        if (!string.IsNullOrEmpty(appSettings.ApplicationInsights.ConnectionString))
        {
            builder.UseAzureMonitor(options =>
            {
                options.ConnectionString = appSettings.ApplicationInsights.ConnectionString;
            });
        }

        return services;
    }
}
