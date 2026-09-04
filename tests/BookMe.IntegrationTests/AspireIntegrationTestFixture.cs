using Aspire.Hosting;
using Aspire.Hosting.Testing;
using BookMe.Infrastructure.Data;
using BookMe.IntegrationTests.TestData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookMe.IntegrationTests;

/// <summary>
/// Boots the real BookMe.AppHost (SQL Server only — Seq/Kafka/UI are skipped via
/// BookMe:TestMode, see AppHost.cs) and drives BookMeAPI over real HTTP.
/// </summary>
public class AspireIntegrationTestFixture : IAsyncLifetime
{
    private static readonly TimeSpan _startupTimeout = TimeSpan.FromMinutes(2);

    private DistributedApplication _app = null!;
    private string _connectionString = null!;

    public async Task InitializeAsync()
    {
        var cancellationToken = CancellationToken.None;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.BookMe_AppHost>(
            ["--BookMe:TestMode=true"],
            cancellationToken
        );

        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        _app = await appHost.BuildAsync(cancellationToken).WaitAsync(_startupTimeout, cancellationToken);
        await _app.StartAsync(cancellationToken).WaitAsync(_startupTimeout, cancellationToken);

        await _app
            .ResourceNotifications.WaitForResourceHealthyAsync("bookme-api", cancellationToken)
            .WaitAsync(_startupTimeout, cancellationToken);

        _connectionString =
            await _app.GetConnectionStringAsync("BookMeDb", cancellationToken)
            ?? throw new InvalidOperationException("BookMeDb connection string was not available.");

        using var dbContext = CreateDbContext();
        SeedData.SeedUsers(dbContext);
    }

    public async Task DisposeAsync()
    {
        await _app.DisposeAsync();
    }

    public HttpClient CreateApiClient() => _app.CreateHttpClient("bookme-api");

    public BookMeContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<BookMeContext>()
            .UseSqlServer(_connectionString)
            .Options;

        return new BookMeContext(options);
    }
}