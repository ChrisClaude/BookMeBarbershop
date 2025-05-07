using System;
using BookMe.Infrastructure.Data;
using BookMeAPI;
using Microsoft.AspNetCore.Hosting;
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
        });
    }

    public Task InitializeAsync()
    {
        return _sqlContainer.StartAsync();
    }

    public new Task DisposeAsync()
    {
        return _sqlContainer.StopAsync();
    }
}
