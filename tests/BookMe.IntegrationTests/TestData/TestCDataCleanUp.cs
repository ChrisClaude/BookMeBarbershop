using BookMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookMe.IntegrationTests.TestData;

public static class TestCDataCleanUp
{
    public static async Task CleanUpDatabaseAsync(BookMeContext bookMeContext)
    {
        await bookMeContext.Bookings.ExecuteDeleteAsync();
        await bookMeContext.TimeSlots.ExecuteDeleteAsync();

        // Get default users
        var testUsers = await bookMeContext
            .Users.Include(x => x.UserRoles)
            .Where(x =>
                x.Email != "jane.doe.customer@test.com" && x.Email != "john.doe.admin@test.com"
            )
            .ToListAsync();

        var userRoles = testUsers.SelectMany(x => x.UserRoles).ToList();
        bookMeContext.UserRoles.RemoveRange(userRoles);
        bookMeContext.Users.RemoveRange(testUsers);
        await bookMeContext.SaveChangesAsync();
        var timeslotCount = await bookMeContext.TimeSlots.CountAsync();
        var bookingCount = await bookMeContext.Bookings.CountAsync();
        var userCount = await bookMeContext.Users.CountAsync(x =>
            x.Email != "jane.doe.customer@test.com" && x.Email != "john.doe.admin@test.com"
        );

        if (timeslotCount > 0 && bookingCount > 0 && userCount > 0)
        {
            throw new Exception("Clean up failed");
        }
    }
}
