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
    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("TestDb1234_")
        .Build();

    public MockHttpContextAccessor MockHttpContext { get; } = new MockHttpContextAccessor();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(
            (context, config) =>
            {
            // TODO: Review this - because the tests are loading configs from appsettings.json, we need to add the test config instead
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
        await _sqlContainer.StartAsync();

        using (var scope = Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<BookMeContext>();

            dbContext.Database.EnsureCreated();

            SeedData.SeedUsers(dbContext);
        }
    }

    public new async Task DisposeAsync()
    {
        await _sqlContainer.StopAsync();
    }
}
