using BookMe.Application.Configurations;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry;

namespace BookMeAPI.Configurations;

public static class LoggingConfiguration
{
    public static IServiceCollection ConfigureSerilog(
        this IServiceCollection services,
        AppSettings appSettings
    )
    {
        var elasticUri = appSettings.Elasticsearch.Uri;
        var environment =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var applicationName = "BookMeAPI";

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProperty("Application", applicationName)
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
            )
            .WriteTo.Conditional(
                evt => !string.IsNullOrEmpty(elasticUri),
                sinkConfig =>
                    sinkConfig
                        .Elasticsearch(
                            new[] { new Uri(elasticUri!) },
                            opts =>
                            {
                                // Data stream configuration
                                opts.DataStream = new DataStreamName(
                                    "logs",
                                    applicationName.ToLower(),
                                    environment.ToLower()
                                );
                            },
                            transport =>
                            {
                                if (!string.IsNullOrEmpty(appSettings.Elasticsearch.ApiKey))
                                {
                                    transport.Authentication(
                                        new ApiKey(appSettings.Elasticsearch.ApiKey)
                                    );
                                }

                                if (
                                    !string.IsNullOrEmpty(appSettings.Elasticsearch.Username)
                                    && !string.IsNullOrEmpty(appSettings.Elasticsearch.Password)
                                )
                                {
                                    transport.Authentication(
                                        new BasicAuthentication(
                                            appSettings.Elasticsearch.Username,
                                            appSettings.Elasticsearch.Password
                                        )
                                    );
                                }
                            }
                        )
                        .WriteTo.Debug()
                        .WriteTo.Conditional(
                            evt => evt.Level == LogEventLevel.Error,
                            sinkConfig =>
                                sinkConfig.File(
                                    path: "logs/errors-.txt",
                                    rollingInterval: RollingInterval.Day
                                )
                        )
            )
            .WriteTo.OpenTelemetry(options =>
            {
                options.Endpoint = appSettings.OpenTelemetry.Seq.LogsUri;
                options.Protocol = OtlpProtocol.HttpProtobuf;
                options.Headers = new Dictionary<string, string>
                {
                    { "X-Seq-ApiKey", appSettings.OpenTelemetry.Seq.ApiKey },
                };
            })
            .WriteTo.ApplicationInsights(
                services.BuildServiceProvider().GetRequiredService<TelemetryConfiguration>(),
                TelemetryConverter.Traces
            )
            .CreateLogger();

        services.AddSerilog();
        return services;
    }
}
