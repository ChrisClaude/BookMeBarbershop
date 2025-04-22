using BookMe.Application.Behaviors;
using BookMe.Application.Commands.Users;
using BookMe.Application.Interfaces.Queries;
using BookMe.Application.Queries;
using BookMe.Application.Validators;
using FluentValidation;
using MediatR;
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

        // Register the command validators for the validator behavior (validators based on FluentValidation library)
        services.AddSingleton<IValidator<CreateOrUpdateUserCommand>, CreateOrUpdateUserCommandValidator>();

        // Register queries
        services.AddScoped<IUserQueries, UserQueries>();

        return services;
    }
}
