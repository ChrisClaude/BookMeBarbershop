using BookMe.Application.Configurations;
using Serilog;

namespace BookMeAPI.Configurations;

public static class CorsConfiguration
{
    public static IServiceCollection ConfigureCors(
        this IServiceCollection services,
        AppSettings appSettings,
        string policyName
    )
    {
        if (appSettings.AllowedCorsOrigins == null)
        {
            throw new ArgumentNullException(
                nameof(appSettings),
                "Allowed CORS origins are not configured"
            );
        }

        for (var i = 0; i < appSettings.AllowedCorsOrigins?.Length; i++)
        {
            Log.Information(
                "Allowed CORS origin {i}: {allowedCorsOrigins[i]}",
                i,
                appSettings.AllowedCorsOrigins[i]
            );
        }

        services.AddCors(options =>
        {
            options.AddPolicy(
                policyName,
                builder =>
                {
                    if (appSettings.AllowedCorsOrigins != null)
                        builder.WithOrigins(appSettings.AllowedCorsOrigins);

                    builder.AllowAnyHeader().AllowAnyMethod();
                }
            );
        });

        return services;
    }
}
