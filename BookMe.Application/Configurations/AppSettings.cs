using System;

namespace BookMe.Application.Configurations;

public class AppSettings
{
  public CacheConfig CacheConfig { get; set; }
  public EventConfig EventConfig { get; set; }
  public ElasticsearchConfig Elasticsearch { get; set; }
  public AzureB2CConfig AzureAdB2C { get; set; }
  public SerilogConfig Serilog { get; set; }
}

#region other config classes

public class ElasticsearchConfig
{
  public string Uri { get; set; }
  public string ApiKey { get; set; }
  public string Username { get; set; }
  public string Password { get; set; }
}

public class SerilogConfig
{
  public bool EnableFileLogging { get; set; }
}

#endregion
