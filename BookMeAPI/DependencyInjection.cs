using BookMe.Application;
using BookMe.Application.Configurations;
using BookMe.Infrastructure;
using Scalar.AspNetCore;

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
        services.AddApplication();
        services.AddInfrastructure(configuration);
        var logger = services.BuildServiceProvider().GetService<ILogger<Program>>();

        ConfigureCors(services, builder.Configuration, logger);
        return builder.Build();
    }

    public static WebApplication ConfigureRequestPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();

        return app;
    }

    private static void ConfigureCors(IServiceCollection services, ConfigurationManager configuration, ILogger logger)
    {
        var allowedCorsOrigins = configuration.GetSection("AllowedCorsOrigins").Get<string[]>();

        for (var i = 0; i < allowedCorsOrigins?.Length; i++)
        {
            logger.LogInformation("{message}", $"Allowed CORS origin {i}: {allowedCorsOrigins[i]}");
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
