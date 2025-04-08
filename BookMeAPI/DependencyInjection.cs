using BookMe.Application;
using BookMe.Application.Configurations;
using BookMe.Infrastructure;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;

internal static class DependencyInjection
{
    private static readonly string _corsPolicyName = "AllowCorsPolicy";

    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        services.Configure<AppSettings>(
            configuration.GetSection("AppSettings"));
        services.AddOpenApi();
        services.AddApplication()
            .AddInfrastructure(configuration);

        ConfigureSerilog(configuration);

        builder.Host.UseSerilog();

        ConfigureCors(services, builder.Configuration);
        return builder.Build();
    }

    private static void ConfigureSerilog(ConfigurationManager configuration)
    {
        var elasticUri = configuration["Elasticsearch:Uri"];
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
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
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                fileSizeLimitBytes: 5242880, // 5MB
                rollOnFileSizeLimit: true,
                shared: true,
                flushToDiskInterval: TimeSpan.FromSeconds(1))
            .WriteTo.Elasticsearch(new[] { new Uri(elasticUri) }, opts =>
            {
                // Data stream configuration
                opts.DataStream = new DataStreamName(
                    "logs",
                    applicationName.ToLower(),
                    environment.ToLower());
            }, transport =>
            {
                if (!string.IsNullOrEmpty(configuration["Elasticsearch:ApiKey"]))
                {
                    transport.Authentication(new ApiKey(configuration["Elasticsearch:ApiKey"]));
                }

                if (!string.IsNullOrEmpty(configuration["Elasticsearch:Username"]) &&
                    !string.IsNullOrEmpty(configuration["Elasticsearch:Password"]))
                {
                    transport.Authentication(new BasicAuthentication(
                        configuration["Elasticsearch:Username"],
                        configuration["Elasticsearch:Password"]));
                }
            })
            .WriteTo.Debug()
            .WriteTo.Conditional(
                evt => evt.Level == LogEventLevel.Error,
                sinkConfig => sinkConfig.File(
                    path: "logs/errors-.txt",
                    rollingInterval: RollingInterval.Day))
            .CreateLogger();
    }

    public static WebApplication ConfigureRequestPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseCors(_corsPolicyName);
        app.UseSerilogRequestLogging();

        app.UseHttpsRedirection();

        return app;
    }

    private static void ConfigureCors(IServiceCollection services, ConfigurationManager configuration)
    {
        var allowedCorsOrigins = configuration.GetSection("AllowedCorsOrigins").Get<string[]>();

        for (var i = 0; i < allowedCorsOrigins?.Length; i++)
        {
            Log.Information("Allowed CORS origin {i}: {allowedCorsOrigins[i]}", i, allowedCorsOrigins[i]);
        }

        services.AddCors(options =>
        {
            options.AddPolicy(_corsPolicyName,
                builder =>
                {
                    if (allowedCorsOrigins != null)
                        builder.WithOrigins(allowedCorsOrigins);

                    builder.AllowAnyHeader()
                           .AllowAnyMethod();
                });
        });
    }
}
