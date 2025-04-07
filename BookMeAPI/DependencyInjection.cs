using BookMe.Application;
using BookMe.Application.Configurations;
using BookMe.Infrastructure;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Scalar.AspNetCore;
using Serilog;

namespace BookMeAPI;

public static class DependencyInjection
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
        Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                    .WriteTo.Elasticsearch(new[] { new Uri("http://localhost:9200") }, opts =>
                    {
                        opts.DataStream = new DataStreamName("logs", "console-example", "demo");
                        opts.BootstrapMethod = BootstrapMethod.Failure;
                        opts.ConfigureChannel = channelOpts =>
                        {
                            channelOpts.BufferOptions = new Elastic.Channels.BufferOptions
                            {
                                ExportMaxConcurrency = 10
                            };
                        };
                    }, transport =>
                    {

                        transport.Authentication(new ApiKey(configuration["ElasticApiKey"])); // ApiKey
                    })
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
