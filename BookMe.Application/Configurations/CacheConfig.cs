namespace BookMe.Application.Configurations;

public class CacheConfig
{
    public CacheType CacheType { get; protected set; }

    public bool Enabled { get; protected set; } = false;

    public int CacheTime { get; protected set; } = 10; // minutes
}
