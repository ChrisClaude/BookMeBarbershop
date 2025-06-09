using BookMe.Application.Configurations;
using BookMe.IntegrationTests.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace BookMe.IntegrationTests.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ReplaceHealthChecksService(this IServiceCollection services)
    {
        // Remove health checks registration to avoid null reference
        var healthChecksBuilder = services.SingleOrDefault(s =>
            s.ServiceType == typeof(HealthCheckService)
        );
        if (healthChecksBuilder != null)
            services.Remove(healthChecksBuilder);

        // Add minimal health checks for testing
        services.AddHealthChecks();
    }

    public static void MockHttpContextAccessor(
        this IServiceCollection services,
        MockHttpContextAccessor mockHttpContext
    )
    {
        var httpContextDescriptor = services.SingleOrDefault(s =>
            s.ServiceType == typeof(IHttpContextAccessor)
        );
        if (httpContextDescriptor != null)
            services.Remove(httpContextDescriptor);

        services.AddSingleton<IHttpContextAccessor>(mockHttpContext);
    }

    public static void AddTestConfiguration(this IServiceCollection services)
    {
        var appSettings = new AppSettings
        {
            AzureAdB2C = new AzureB2CConfig
            {
                Instance = "https://test-instance.b2clogin.com",
                Domain = "test-domain.onmicrosoft.com",
                TenantId = "test-tenant-id",
                ClientId = "test-client-id",
                SignUpSignInPolicyId = "B2C_1_test_policy",
            },
            Elasticsearch = new ElasticsearchConfig { Uri = "http://localhost:9200" },
            OpenTelemetry = new OpenTelemetryConfig
            {
                Seq = new OpenTelemetryConfig.SeqConfig
                {
                    LogsUri = "http://localhost:5341/ingest/otlp/v1/logs",
                    TracesUri = "http://localhost:5341/ingest/otlp/v1/traces",
                },
            },
            ApplicationInsights = new ApplicationInsightsConfig { ConnectionString = "" },
        };

        services.RemoveAll(typeof(IOptionsSnapshot<AppSettings>));
        services.RemoveAll(typeof(IOptions<AppSettings>));
        services.AddSingleton(Options.Create(appSettings));
    }
}
