using BookMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BookMeAPI.Configurations;

public static class MigrationConfiguration
{
    public static void MigrateDatabase(this WebApplication app)
    {
        Log.Information("Migrating database...");
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BookMeContext>();
        db.Database.Migrate();
        Log.Information("Database migrated successfully.");
    }
}
