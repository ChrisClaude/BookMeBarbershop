using System;
using AspNetCoreRateLimit;

namespace BookMeAPI.Configurations;

public static class AspNetCoreRateLimitConfiguration
{
    public static IServiceCollection ConfigureAspNetCoreRateLimit(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // docs for help: https://github.com/stefanprodan/AspNetCoreRateLimit/wiki/IpRateLimitMiddleware#setup
        // configure ip rate limiting
        services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
        services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));

        // register the counter store
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>(); // Or RedisIpPolicyStore

        // register the IP-based rate limiter
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>(); // Or RedisRateLimitCounterStore

        // The following two are mandatory for the IpRateLimiting and ClientRateLimiting
        // (if you use ClientRateLimiting)
        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>(); // Or ConcurrentProcessingStrategy
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        // inject counter and rules stores
        services.AddHttpContextAccessor(); // Needed for IpRateLimiting

        return services;
    }
}
