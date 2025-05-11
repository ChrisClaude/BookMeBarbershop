namespace BookMe.IntegrationTests.TestData;

public static class TestConfig
{
    public static Dictionary<string, string?> GetConfiguration()
    {
        return new Dictionary<string, string?>
            {
                {"ConnectionStrings:BookMeDb", "TestConnectionString"},
                {"AppSettings:AllowedCorsOrigins:0", "http://localhost:3000"},
                {"AppSettings:CacheConfig:CacheType", "memory"},
                {"AppSettings:CacheConfig:Enabled", "true"},
                {"AppSettings:CacheConfig:CacheTime", "10"},
                {"AppSettings:EventConfig:Server", "localhost:9092"},
                {"AppSettings:EventConfig:Topic", "bookme-test"},
                {"AppSettings:Elasticsearch:Uri", "http://localhost:9200"},
                {"AppSettings:AzureAdB2C:Instance", "https://test-instance.b2clogin.com"},
                {"AppSettings:AzureAdB2C:Domain", "test-domain.onmicrosoft.com"},
                {"AppSettings:AzureAdB2C:TenantId", "test-tenant-id"},
                {"AppSettings:AzureAdB2C:ClientId", "test-client-id"},
                {"AppSettings:AzureAdB2C:SignUpSignInPolicyId", "B2C_1_test_policy"},
                {"AppSettings:OpenTelemetry:Seq:LogsUri", "http://localhost:5341/ingest/otlp/v1/logs"},
                {"AppSettings:OpenTelemetry:Seq:TracesUri", "http://localhost:5341/ingest/otlp/v1/traces"},
                // Additional health check related configurations
                {"AppSettings:Elasticsearch:ApiKey", "test-api-key"},
                {"AppSettings:Elasticsearch:Username", "test-username"},
                {"AppSettings:Elasticsearch:Password", "test-password"},
                {"AppSettings:OpenTelemetry:Seq:ApiKey", "test-seq-api-key"}
            };
    }
}
