using BookMe.Infrastructure.Data;
using BookMe.IntegrationTests.Extensions;
using BookMe.IntegrationTests.Mocks;
using BookMe.IntegrationTests.TestData;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace BookMe.IntegrationTests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // container is static to share across test instances
    private static readonly MsSqlContainer _sqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("TestDb1234_")
        .WithEnvironment("MSSQL_MEMORY_LIMIT_MB", "1024")
        .WithEnvironment("MSSQL_TELEMETRY_ENABLED", "FALSE")
        .Build();

    private static bool _containerStarted = false;
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public MockHttpContextAccessor MockHttpContext { get; } = new MockHttpContextAccessor();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Add test configuration
        builder.ConfigureAppConfiguration(
            (context, config) =>
            {
                config.AddInMemoryCollection(TestConfig.GetConfiguration());
            }
        );

        builder.ConfigureTestServices(services =>
        {
            var descriptorType = typeof(DbContextOptions<BookMeContext>);

            var descriptor = services.SingleOrDefault(s => s.ServiceType == descriptorType);

            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<BookMeContext>(options =>
            {
                options.UseSqlServer(_sqlContainer.GetConnectionString());
            });

            services.ReplaceHealthChecksService();
            services.MockHttpContextAccessor(MockHttpContext);
            services.AddTestConfiguration();
        });
    }

    public async Task InitializeAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (!_containerStarted)
            {
                await _sqlContainer.StartAsync();
                _containerStarted = true;
            }
        }
        finally
        {
            _semaphore.Release();
        }

        using (var scope = Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<BookMeContext>();

            await dbContext.Database.EnsureCreatedAsync();

            if (!await dbContext.Users.AnyAsync())
            {
                SeedData.SeedUsers(dbContext);
            }
        }
    }

    public new async Task DisposeAsync()
    {
        // Don't stop the container after each test class
        // Only clean up resources specific to this instance
    }

    // This method is called at the end of all tests to stop the container
    // as opposed to after each test class
    public static async Task CleanupContainerAsync()
    {
        if (_containerStarted)
        {
            await _sqlContainer.StopAsync();
            _containerStarted = false;
        }
    }
}
