
using BookMe.Application;
using BookMe.Application.Configurations;
using BookMe.Infrastructure;
using BookMeAPI.Configurations;
using BookMeAPI.MiddleWare;
using Elastic.Transport;
using Scalar.AspNetCore;
using Serilog;

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

        services
            .AddOpenApi()
            .AddApplication()
            .AddInfrastructure(configuration)
            .ConfigureAuthentication(configuration)
            .ConfigureSerilog(appSettings)
            .ConfigureCors(appSettings, _corsPolicyName);

        builder.Host.UseSerilog();

        // Add exception handler and problem details
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return builder.Build();
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
        app.UseExceptionHandler();

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
    #endregion
}
