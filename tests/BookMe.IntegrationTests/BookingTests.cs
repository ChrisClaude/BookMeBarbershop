using BookMe.Application.Common.Bookings.Dtos;
using BookMe.Application.Interfaces.Queries;
using BookMeAPI.Controllers;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookMe.IntegrationTests;

public class BookingTests : BaseIntegrationTest
{

    public BookingTests(IntegrationTestWebAppFactory factory) : base(factory)
    {

    }

    [Fact]
    public async Task CreateTimeSlotsShouldSucceed()
    {
        // Arrange
        _mockHttpContext.SetCustomerUser();
        var mediator = _scope.ServiceProvider.GetRequiredService<IMediator>();
        var timeSlotQueries = _scope.ServiceProvider.GetRequiredService<ITimeSlotQueries>();
        var bookingController = new BookingController(mediator, timeSlotQueries);

        var createTimeSlotsCommand = new CreateTimeSlotsDto
        {
            StartDateTime = DateTime.UtcNow.AddDays(10).AddHours(1),
            EndDateTime = DateTime.UtcNow.AddDays(10).AddHours(2),
        };

        // Act
        var result = await bookingController.CreateTimeSlotsAsync(createTimeSlotsCommand);

        // Assert
        result.ValidateCreatedResult();

        var timeSlots = await _bookMeContext.TimeSlots
            .ToListAsync();


        timeSlots.Should().HaveCount(1);
    }
}
