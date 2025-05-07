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

        services.AddAuthorizationBuilder()
            .AddPolicy(Policy.CUSTOMER, policy => policy.Requirements.Add(new CustomerRequirement()));

        services.AddSingleton<IAuthorizationHandler, AdminAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, CustomerAuthorizationHandler>();

        return services;
    }
}
