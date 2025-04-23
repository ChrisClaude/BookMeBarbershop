using BookMe.Application.Entities;
using BookMe.Application.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookMe.Infrastructure.Data;

/// <summary>
/// Provides data seeding functionality for the BookMe database context.
/// Contains methods for seeding initial/default data for various entities.
/// </summary>
public static class SeedDataProvider
{
    public static void SeedData(this ModelBuilder modelBuilder)
    {
        SeedRoles(modelBuilder);
    }

    private static void SeedRoles(ModelBuilder modelBuilder)
    {

        var roles = new List<Role>
        {
            new() { Id = Guid.Parse("6B29FC40-CA47-1067-B31D-00DD010662FD"), Name = RoleName.Admin },
            new() { Id = Guid.Parse("6B29FC40-CA47-1067-B31D-00DD010662FC"), Name = RoleName.Customer }
        };
        modelBuilder.Entity<Role>().HasData(roles);
    }
}
