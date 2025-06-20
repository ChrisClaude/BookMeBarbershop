using BookMe.Application.Caching;
using BookMe.Application.Configurations;
using BookMe.Application.Events;
using BookMe.Application.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace BookMeAPI.HealthChecks;

public class CustomHealthCheck : IHealthCheck
{
    private readonly ICacheManager _cacheManager;
    private readonly IEventPublisher _eventPublisher;
    private readonly AppSettings _appSettings;

    public CustomHealthCheck(
        ICacheManager cacheManager,
        IEventPublisher eventPublisher,
        IOptionsSnapshot<AppSettings> appSettings
    )
    {
        _cacheManager = cacheManager;
        _eventPublisher = eventPublisher;
        _appSettings = appSettings.Value;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        var isHealthy = true;
        var data = new Dictionary<string, object>();

        try
        {
            var cacheKey = new CacheKey(
                $"health_check_{DateTime.UtcNow.Ticks}",
                _appSettings.CacheConfig
            );

            await _cacheManager.AddAsync(cacheKey, "test");
            await _cacheManager.GetAsync<string>(cacheKey, out var cacheValue);
            await _cacheManager.RemoveAsync(cacheKey.Key);

            data.Add("Cache", cacheValue == "test" ? "Working" : "Failed");
        }
        catch (Exception ex)
        {
            isHealthy = false;
            data.Add("Cache", $"Error: {ex.Message}");
        }

        try
        {
            if (_appSettings.EventConfig.Enabled)
            {
                // Check event publisher
                await _eventPublisher.PublishAsync(new HealthCheckEvent());
                data.Add("EventPublisher", "Working");
            }
        }
        catch (Exception ex)
        {
            isHealthy = false;
            data.Add("EventPublisher", $"Error: {ex.Message}");
        }

        // Check elasticsearch configuration
        try
        {
            if (string.IsNullOrEmpty(_appSettings.Elasticsearch?.Uri))
            {
                data.Add("Elasticsearch", "Error: Missing Elasticsearch URI");
                isHealthy = false;
            }
            else
            {
                data.Add("Elasticsearch", "Valid");
            }
        }
        catch (Exception ex)
        {
            isHealthy = false;
            data.Add("Elasticsearch", $"Error: {ex.Message}");
        }

        var healthyMessage = $"All custom checks ({string.Join(", ", data.Keys)}) passed";
        return isHealthy
            ? HealthCheckResult.Healthy(healthyMessage, data)
            : HealthCheckResult.Unhealthy("One or more custom checks failed", null, data);
    }
}

public class HealthCheckEvent : IEvent
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}
