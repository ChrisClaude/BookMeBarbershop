using System;
using BookMe.Infrastructure.Data;
using BookMe.IntegrationTests.Mocks;
using BookMe.IntegrationTests.TestData;
using BookMeAPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
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

            MockHttpContextAccessor(services);
        });
    }

    private void MockHttpContextAccessor(IServiceCollection services)
    {
        var httpContextDescriptor = services.SingleOrDefault(
            s => s.ServiceType == typeof(IHttpContextAccessor));
        if (httpContextDescriptor != null)
            services.Remove(httpContextDescriptor);

        services.AddSingleton<IHttpContextAccessor>(MockHttpContext);
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

    public new Task DisposeAsync()
    {
        return _sqlContainer.StopAsync();
    }
}
