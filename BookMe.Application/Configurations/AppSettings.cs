using System;

namespace BookMe.Application.Configurations;

public class AppSettings
{
  public CacheConfig CacheConfig { get; set; }
  public EventConfig EventConfig { get; set; }

}
