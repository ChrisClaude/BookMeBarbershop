using BookMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookMe.IntegrationTests.TestData;

public static class TestCDataCleanUp
{
    public static async Task CleanUpDatabaseAsync(BookMeContext bookMeContext)
    {
        await bookMeContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [Bookings]");
        await bookMeContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [TimeSlots]");
        await bookMeContext.Database.ExecuteSqlRawAsync(
            "DELETE FROM [Users] WHERE [Email] NOT IN ('jane.doe.customer@test.com', 'john.doe.admin@test.com')"
        );
    }
}
