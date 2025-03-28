using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BookMe.Infrastructure.Data;

namespace BookMe.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BookMeContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("BookMeDb"));
        });

        return services;
    }
}
