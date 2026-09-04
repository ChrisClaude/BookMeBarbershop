using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

// Flips this AppHost into a minimal graph for BookMe.IntegrationTests:
// SQL Server only, ephemeral (no data volume / fixed ports), no Seq/Kafka/UI.
// Set via DistributedApplicationTestingBuilder.CreateAsync(["--BookMe:TestMode=true"]).
var isTestMode = builder.Configuration.GetValue<bool>("BookMe:TestMode");

// ---------------------------------------------------------------------------
// Infrastructure dependencies
//
// Host ports are pinned to the values already baked into the API's
// appsettings (1433 / 5341 / 9092) so the app works whether its connection
// details come from Aspire injection or from appsettings.secrets.json.
// ---------------------------------------------------------------------------

// SQL Server — primary datastore. EF Core migrations run on API startup.
// azure-sql-edge ships native arm64 images; mcr.microsoft.com/mssql/server is
// amd64-only and gets OOM-killed under emulation on Apple Silicon.
var sqlPassword = builder.AddParameter("sql-password", "FACEpass107_=0x", secret: true);

var sqlBuilder = isTestMode
    ? builder.AddSqlServer("sql", sqlPassword)
    : builder
        .AddSqlServer("sql", sqlPassword, port: 1433)
        .WithDataVolume("bookme-sqldata")
        .WithLifetime(ContainerLifetime.Persistent);

var sql = sqlBuilder.WithImageRegistry("mcr.microsoft.com").WithImage("azure-sql-edge").WithImageTag("latest");

var bookMeDb = sql.AddDatabase("BookMeDb");

// ---------------------------------------------------------------------------
// Applications
// ---------------------------------------------------------------------------

var api = isTestMode
    ? builder.AddProject<Projects.BookMeAPI>("bookme-api").WithEnvironment("ASPNETCORE_ENVIRONMENT", "Testing")
    : builder
        .AddProject<Projects.BookMeAPI>("bookme-api")
        .WithHttpsEndpoint(port: 6002, name: "https")
        .WithHttpEndpoint(port: 6003, name: "http");

api = api.WithReference(bookMeDb) // injects ConnectionStrings__BookMeDb
    .WaitFor(bookMeDb)            // don't start migrations until SQL Server is ready
    .WithHttpHealthCheck("/healthz");

if (!isTestMode)
{
    // Seq — structured log + OTLP trace sink (matches AppSettings:OpenTelemetry:Seq).
    // UI: http://localhost:5341
    var seq = builder.AddSeq("seq", port: 5341)
        .WithDataVolume("bookme-seqdata")
        .WithLifetime(ContainerLifetime.Persistent);

    // Kafka — domain event bus. Off by default via AppSettings:EventConfig:Enabled;
    // flip that to true to publish events to this broker.
    var kafka = builder.AddKafka("kafka", port: 9092).WithLifetime(ContainerLifetime.Persistent);

    api = api.WithReference(kafka).WaitFor(seq);

    var ui = builder.AddNpmApp("bookme-ui", "../bookme-ui")
        .WithHttpEndpoint(port: 3000, env: "PORT")
        .WithExternalHttpEndpoints()
        .WithReference(api)
        .WaitFor(api);
}

builder.Build().Run();