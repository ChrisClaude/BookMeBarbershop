using BookMe.Application.Caching;
using BookMe.Application.Configurations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace BookMe.Infrastructure.Caching;

public class MemoryCacheManager : ICacheManager
{
    private readonly IMemoryCache _memoryCache;
    private readonly IOptionsSnapshot<AppSettings> _appSettings;

    public MemoryCacheManager(IMemoryCache memoryCache,
        IOptionsSnapshot<AppSettings> appSettings)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
    }

    public Task AddAsync<T>(CacheKey cacheKey, T value)
    {
        ArgumentNullException.ThrowIfNull(cacheKey);

        var cacheTime = cacheKey.CacheTime;
        var options = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(cacheTime))
            .SetSlidingExpiration(TimeSpan.FromMinutes(cacheTime / 2));

        _memoryCache.Set(cacheKey.Key, value, options);

        return Task.CompletedTask;
    }

    public Task<bool> GetAsync<T>(CacheKey cacheKey, out T result)
    {
        ArgumentNullException.ThrowIfNull(cacheKey);
        ArgumentException.ThrowIfNullOrEmpty(cacheKey.Key);

        var success = _memoryCache.TryGetValue(cacheKey.Key, out T value);
        result = value!;

        return Task.FromResult(success);
    }

    public Task RemoveAsync(string cacheKey)
    {
        ArgumentException.ThrowIfNullOrEmpty(cacheKey);

        _memoryCache.Remove(cacheKey);

        return Task.CompletedTask;
    }

    public Task RemoveByPrefixAsync(string prefix)
    {
        ArgumentException.ThrowIfNullOrEmpty(prefix);

        // Since IMemoryCache doesn't support pattern-based removal,
        // we would need to maintain a separate collection of keys
        // This is a limitation of IMemoryCache

        return Task.CompletedTask;
    }
}
