using System;
using BookMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookMeAPI.Configurations;

public static class MigrationConfiguration
{
    public static void MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BookMeContext>();
        db.Database.Migrate();
    }
}
