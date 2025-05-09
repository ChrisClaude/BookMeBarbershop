using System;
using BookMe.Application.Entities;
using BookMe.Application.Enums;
using BookMe.Infrastructure.Data;

namespace BookMe.IntegrationTests.TestData;

public static class SeedData
{
    public static void SeedUsers(BookMeContext context)
    {
        var admin = new User
        {
            Id = Guid.NewGuid(),
            Name = "John",
            Surname = "Doe",
            Email = "john.doe.admin@test.com",
            PhoneNumber = "1234567890",
            UserRoles = new List<UserRole>
            {
                new() { RoleId = DefaultRoles.AdminId }
            },
        };

        var customer = new User
        {
            Id = Guid.NewGuid(),
            Name = "Jane",
            Surname = "Doe",
            Email = "jane.doe.customer@test.com",
            PhoneNumber = "1234567890",
            UserRoles = new List<UserRole>
            {
                new() { RoleId = DefaultRoles.CustomerId }
            },
        };

        context.Users.AddRange(admin, customer);
        context.SaveChanges();
    }
}
