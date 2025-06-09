using BookMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookMe.IntegrationTests.TestData;

public static class TestDataCleanUp
{
    public static async Task CleanUpDatabaseAsync(BookMeContext bookMeContext)
    {
        // // Use ExecuteDeleteAsync instead of Remove + SaveChanges
        await bookMeContext.Bookings.ExecuteDeleteAsync();
        await bookMeContext.TimeSlots.ExecuteDeleteAsync();

        // Get default users
        var testUsers = await bookMeContext
            .Users.Include(x => x.UserRoles)
            .Where(x =>
                x.Email != "jane.doe.customer@test.com" && x.Email != "john.doe.admin@test.com"
            )
            .ToListAsync();

        if (testUsers.Count > 0)
        {
            // Use direct SQL to avoid concurrency issues
            var userIds = testUsers.Select(u => u.Id).ToList();

            // Delete related UserRoles first
            await bookMeContext.Database.ExecuteSqlRawAsync(
                "DELETE FROM UserRoles WHERE UserId IN ({0})",
                string.Join(",", userIds)
            );

            // Then delete Users
            await bookMeContext.Database.ExecuteSqlRawAsync(
                "DELETE FROM Users WHERE Id IN ({0})",
                string.Join(",", userIds)
            );
        }

        // Verify cleanup
        var timeslotCount = await bookMeContext.TimeSlots.CountAsync();
        var bookingCount = await bookMeContext.Bookings.CountAsync();
        var userCount = await bookMeContext.Users.CountAsync(x =>
            x.Email != "jane.doe.customer@test.com" && x.Email != "john.doe.admin@test.com"
        );

        if (timeslotCount > 0 || bookingCount > 0 || userCount > 0)
        {
            throw new Exception("Clean up failed");
        }
    }
}
