using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BookMe.Application.Extensions;

public static class ValidationServiceExtensions
{
    /// <summary>
    /// Register the command validators for the validator behavior (validators based on FluentValidation library)
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddValidationService(this IServiceCollection services)
    {
        // Register all validators in the assembly
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Singleton);

        return services;
    }
}
