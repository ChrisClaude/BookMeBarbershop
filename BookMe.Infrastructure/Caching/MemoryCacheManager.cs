using System;
using BookMe.Application.Caching;
using BookMe.Application.Configurations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace BookMe.Infrastructure.Caching;

public class MemoryCacheManager(IMemoryCache memoryCache,
    IOptionsSnapshot<AppSettings> appSettings)
    : ICacheManager
{
    public Task AddAsync(string prefix)
    {
        memoryCache.Set(prefix, prefix, TimeSpan.FromMinutes(appSettings.Value.CacheConfig.CacheTime));
        return Task.CompletedTask;
    }

    public Task<bool> GetAsync<T>(CacheKey cacheKey, out T result)
    {
        var containsValue = memoryCache.TryGetValue(cacheKey.Key, out result);
        return Task.FromResult(containsValue);
    }

    public Task RemoveAsync(string cacheKey)
    {
        memoryCache.Remove(cacheKey);
        return Task.CompletedTask;
    }
}
