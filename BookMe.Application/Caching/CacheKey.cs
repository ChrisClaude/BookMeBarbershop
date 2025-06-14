using BookMe.Application.Configurations;

namespace BookMe.Application.Caching;

public class CacheKey
{
    #region Ctor

    /// <summary>
    /// Initialize a new instance with key and prefixes
    /// </summary>
    /// <param name="key">Key</param>
    public CacheKey(string key, CacheConfig cacheConfig)
    {
        Key = key;
        CacheTime = cacheConfig.CacheType switch
        {
            CacheType.Memory => 10,
            CacheType.SqlServer => 10,
            CacheType.Redis => 10,
            CacheType.RedisSynchronizedMemory => 10,
            _ => 10,
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a cache key
    /// </summary>
    public string Key { get; protected set; }

    /// <summary>
    /// Gets or sets a cache time in minutes
    /// </summary>
    public int CacheTime { get; set; }

    #endregion
}
