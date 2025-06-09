using BookMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class MigrationScript
{
    public static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.Production.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<BookMeContext>();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("BookMeDb"));

        using var context = new BookMeContext(optionsBuilder.Options);
        Console.WriteLine("Applying migrations...");
        context.Database.Migrate();
        Console.WriteLine("Migrations applied successfully.");
    }
}
