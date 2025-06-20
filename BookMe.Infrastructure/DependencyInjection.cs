using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BookMe.Infrastructure.Data;
using BookMe.Application.Interfaces;
using BookMe.Infrastructure.Events;
using BookMe.Application.Caching;
using BookMe.Infrastructure.Caching;
using BookMe.Infrastructure.SMS;

namespace BookMe.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<BookMeContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("BookMeDb"));
        });

        services.AddScoped(typeof(IRepository<>), typeof(EntityRepository<>));
        services.AddScoped<IEventPublisher, KafkaProducer>();
        services.AddScoped<ITransactionManager, TransactionManager>();
        services.AddTransient<ICacheManager, MemoryCacheManager>();
        services.AddSingleton<ITwilioSmsService, TwilioSmsService>();

        return services;
    }
}
