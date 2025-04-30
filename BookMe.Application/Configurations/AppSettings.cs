using System;

namespace BookMe.Application.Configurations;

public class AppSettings
{
    public CacheConfig CacheConfig { get; set; }
    public EventConfig EventConfig { get; set; }
    public ElasticsearchConfig Elasticsearch { get; set; }
    public AzureB2CConfig AzureAdB2C { get; set; }
    public SerilogConfig Serilog { get; set; }
    public OpenTelemetryConfig OpenTelemetry { get; set; }
    public string[] AllowedCorsOrigins { get; set; }
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

public class OpenTelemetryConfig
{
    public SeqConfig Seq { get; set; }

    public class SeqConfig
    {
        public string Uri { get; set; }
        public string ApiKey { get; set; }
    }
}

#endregion
