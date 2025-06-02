using System;

namespace BookMe.IntegrationTests;

public class TimeSlotQueryTests : BaseIntegrationTest
{
    public TimeSlotQueryTests(IntegrationTestWebAppFactory factory)
        : base(factory) { }

    // TODO: Add a test whereby an admin view a list of timeslots and booked timeslots should show as not available while available timeslots should show as available
}
