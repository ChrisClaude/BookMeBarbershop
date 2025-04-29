using System;
using BookMe.Application.Configurations;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

internal static class OpenTelemetryConfiguration
{

    public static WebApplicationBuilder UseOpenTelemetry(this WebApplicationBuilder builder)
    {
        var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService("BookMeAPI")
                .AddAttributes(new Dictionary<string, object>()
                {
                    {"Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT" ) ?? "Development"}
                }));

            options.IncludeScopes = true;
            options.IncludeFormattedMessage = true;

            options.AddOtlpExporter(opt =>
            {
                opt.Endpoint = new Uri(appSettings.OpenTelemetry.Endpoint);
                opt.Protocol = OtlpExportProtocol.HttpProtobuf;

            });
        });
        return builder;
    }
}
