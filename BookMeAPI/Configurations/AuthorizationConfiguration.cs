using System;
using BookMeAPI.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace BookMeAPI.Configurations;

public static class AuthorizationConfiguration
{
    public static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
    {

        services.AddAuthorizationBuilder()
            .AddPolicy(Policy.ADMIN, policy => policy.Requirements.Add(new AdminRequirement(true)));

        services.AddSingleton<IAuthorizationHandler, AdminAuthorizationHandler>();

        return services;
    }
}
