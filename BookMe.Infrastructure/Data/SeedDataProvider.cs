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
            new() { Id = DefaultRoles.AdminId, Name = RoleName.ADMIN },
            new() { Id = DefaultRoles.CustomerId, Name = RoleName.CUSTOMER }
        };
        modelBuilder.Entity<Role>().HasData(roles);
    }


}
