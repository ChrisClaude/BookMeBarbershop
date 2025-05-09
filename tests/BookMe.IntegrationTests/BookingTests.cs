using BookMe.Application.Common.Bookings.Dtos;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Entities;
using BookMe.Application.Enums;
using BookMe.Application.Interfaces.Queries;
using BookMe.Application.Mappings;
using BookMe.IntegrationTests.TestData;
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
    public async Task CreateTimeSlotsShouldSucceedAsync()
    {
        // Arrange
        var adminUser = await _bookMeContext.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstAsync(x => x.UserRoles.Any(y => y.RoleId == DefaultRoles.AdminId));

        _mockHttpContext.SetUser(adminUser.MapToDto());
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
        result.ValidateOkResult<TimeSlotDto>(timeSlot =>
        {
            timeSlot.Start.Should().Be(createTimeSlotsCommand.StartDateTime);
            timeSlot.End.Should().Be(createTimeSlotsCommand.EndDateTime);
            timeSlot.IsAvailable.Should().BeTrue();
            timeSlot.Id.Should().NotBeEmpty();
        });

        var timeSlots = await _bookMeContext.TimeSlots
            .ToListAsync();

        timeSlots.Should().HaveCount(1);
    }
}
