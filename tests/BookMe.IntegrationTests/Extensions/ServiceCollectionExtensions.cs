using System;
using BookMe.IntegrationTests.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BookMe.IntegrationTests.Extensions;

public static class ServiceCollectionExtensions
{

    public static void ReplaceHealthChecksService(this IServiceCollection services)
    {
        // Remove health checks registration to avoid null reference
        var healthChecksBuilder = services.SingleOrDefault(
            s => s.ServiceType == typeof(HealthCheckService));
        if (healthChecksBuilder != null)
            services.Remove(healthChecksBuilder);

        // Add minimal health checks for testing
        services.AddHealthChecks();
    }

    public static void MockHttpContextAccessor(this IServiceCollection services, MockHttpContextAccessor mockHttpContext)
    {
        var httpContextDescriptor = services.SingleOrDefault(
            s => s.ServiceType == typeof(IHttpContextAccessor));
        if (httpContextDescriptor != null)
            services.Remove(httpContextDescriptor);

        services.AddSingleton<IHttpContextAccessor>(mockHttpContext);
    }

}
