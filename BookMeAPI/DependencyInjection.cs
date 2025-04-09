using BookMe.Application;
using BookMe.Application.Configurations;
using BookMe.Infrastructure;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;

internal static class DependencyInjection
{
    private static readonly string _corsPolicyName = "AllowCorsPolicy";
    private static AppSettings _appSettings = new();

    #region ConfigureServices
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        _appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();

        services.Configure<AppSettings>(
            configuration.GetSection("AppSettings"));
        services.AddOpenApi();
        services.AddApplication()
            .AddInfrastructure(configuration);

        ConfigureSerilog(_appSettings);

        builder.Host.UseSerilog();

        ConfigureCors(services, builder.Configuration);
        return builder.Build();
    }
    #endregion

    private static void ConfigureAuthentication(IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(options =>
            {
                configuration.Bind("AzureAdB2C", options);
                options.TokenValidationParameters.NameClaimType = "name";
            }, options => { configuration.Bind("AzureAdB2C", options); });
    }

    #region Logging
    private static void ConfigureSerilog(AppSettings appSettings)
    {
        var elasticUri = appSettings.Elasticsearch.Uri;
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
            .WriteTo.Conditional(
                evt => appSettings.Serilog.EnableFileLogging,
                sinkConfig => sinkConfig.File(
                    path: "logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    fileSizeLimitBytes: 5242880, // 5MB
                    rollOnFileSizeLimit: true,
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(1)))
            .WriteTo.Conditional(
                evt => !string.IsNullOrEmpty(elasticUri),
                sinkConfig => sinkConfig.Elasticsearch(new[] { new Uri(elasticUri!) }, opts =>
            {
                // Data stream configuration
                opts.DataStream = new DataStreamName(
                    "logs",
                    applicationName.ToLower(),
                    environment.ToLower());
            }, transport =>
            {
                if (!string.IsNullOrEmpty(appSettings.Elasticsearch.ApiKey))
                {
                    transport.Authentication(new ApiKey(appSettings.Elasticsearch.ApiKey));
                }

                if (!string.IsNullOrEmpty(appSettings.Elasticsearch.Username) &&
                    !string.IsNullOrEmpty(appSettings.Elasticsearch.Password))
                {
                    transport.Authentication(new BasicAuthentication(
                        appSettings.Elasticsearch.Username,
                        appSettings.Elasticsearch.Password));
                }
            })
            .WriteTo.Debug()
            .WriteTo.Conditional(
                evt => evt.Level == LogEventLevel.Error,
                sinkConfig => sinkConfig.File(
                    path: "logs/errors-.txt",
                    rollingInterval: RollingInterval.Day)))
            .CreateLogger();
    }

    #endregion

    #region ConfigureRequestPipeline
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
    #endregion

    #region ConfigureCors
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
    #endregion
}
