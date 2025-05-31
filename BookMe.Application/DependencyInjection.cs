using BookMe.Application.Behaviors;
using BookMe.Application.Extensions;
using BookMe.Application.Interfaces;
using BookMe.Application.Interfaces.Queries;
using BookMe.Application.Queries;
using BookMe.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BookMe.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

            // Order matters
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(AuthRequestBehavior<,>));
            config.AddOpenBehavior(typeof(ValidatorBehavior<,>));
            config.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });

        services.AddValidationService();

        // Register queries
        services.AddScoped<IUserQueries, UserQueries>();
        services.AddScoped<ITimeSlotQueries, TimeSlotQueries>();

        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
