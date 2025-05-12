using BookMe.Application.Commands.Bookings;
using BookMe.Application.Common.Bookings.Dtos;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Common.Dtos.Bookings;
using BookMe.Application.Entities;
using BookMe.Application.Enums;
using BookMe.Application.Interfaces.Queries;
using BookMe.Application.Mappings;
using BookMe.IntegrationTests.TestData;
using BookMeAPI.Controllers;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookMe.IntegrationTests;

public class BookingTests : BaseIntegrationTest
{
    private UserDto adminUser;
    private UserDto customerUser;

    public BookingTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        adminUser = _bookMeContext.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .First(x => x.UserRoles.Any(y => y.RoleId == DefaultRoles.AdminId))
            .MapToDto();

        customerUser = _bookMeContext.Users
        .Include(x => x.UserRoles)
        .ThenInclude(x => x.Role)
        .First(x => x.UserRoles.Any(y => y.RoleId == DefaultRoles.CustomerId))
        .MapToDto();
    }

    #region CreateTimeSlot tests
    [Fact]
    public async Task CreateTimeSlotsShouldSucceedAsync()
    {
        // Arrange
        _mockHttpContext.SetUser(adminUser);
        var mediator = _scope.ServiceProvider.GetRequiredService<IMediator>();
        var timeSlotQueries = _scope.ServiceProvider.GetRequiredService<ITimeSlotQueries>();
        var bookingController = new BookingController(mediator, timeSlotQueries);

        var createTimeSlotsRequest = new CreateTimeSlotsDto
        {
            StartDateTime = DateTime.UtcNow.AddDays(10).AddHours(1),
            EndDateTime = DateTime.UtcNow.AddDays(10).AddHours(2),
        };

        // Act
        var result = await bookingController.CreateTimeSlotsAsync(createTimeSlotsRequest);

        // Assert
        result.ValidateOkResult<TimeSlotDto>(timeSlot =>
        {
            timeSlot.Start.Should().Be(createTimeSlotsRequest.StartDateTime);
            timeSlot.End.Should().Be(createTimeSlotsRequest.EndDateTime);
            timeSlot.IsAvailable.Should().BeTrue();
            timeSlot.Id.Should().NotBeEmpty();
        });

        var timeSlots = await _bookMeContext.TimeSlots
            .ToListAsync();

        timeSlots.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateTimeSlotsWithNonAdminUser_ShouldNotSucceedAsync()
    {
        // Arrange
        var customerUser = await _bookMeContext.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstAsync(x => x.UserRoles.Any(y => y.RoleId == DefaultRoles.CustomerId));

        _mockHttpContext.SetUser(customerUser.MapToDto());
        var mediator = _scope.ServiceProvider.GetRequiredService<IMediator>();
        var timeSlotQueries = _scope.ServiceProvider.GetRequiredService<ITimeSlotQueries>();
        var bookingController = new BookingController(mediator, timeSlotQueries);

        var createTimeSlotsRequest = new CreateTimeSlotsDto
        {
            StartDateTime = DateTime.UtcNow.AddDays(10).AddHours(1),
            EndDateTime = DateTime.UtcNow.AddDays(10).AddHours(2),
        };

        // Act & Assert should throw exception because user is not admin
        var exception = await Assert.ThrowsAsync<ValidationException>(() => bookingController.CreateTimeSlotsAsync(createTimeSlotsRequest));

        exception.Message.Should().Be($"Validation failed: \n -- UserDTo: User {customerUser.Id} is not an admin");

        var timeSlots = await _bookMeContext.TimeSlots
            .ToListAsync();

        timeSlots.Should().HaveCount(0);
    }
    #endregion

    #region BookTimeSlot tests
    [Fact]
    public async Task BookTimeSlotShouldSucceedAsync()
    {
        // Arrange
        _mockHttpContext.SetUser(adminUser);
        var mediator = _scope.ServiceProvider.GetRequiredService<IMediator>();
        var timeSlotQueries = _scope.ServiceProvider.GetRequiredService<ITimeSlotQueries>();
        var bookingController = new BookingController(mediator, timeSlotQueries);

        var createTimeSlotsCommand = new CreateTimeSlotCommand
       (DateTime.UtcNow.AddDays(10).AddHours(1),
             DateTime.UtcNow.AddDays(10).AddHours(2)
       );


        var creationResult = await mediator.Send(createTimeSlotsCommand);

        var timeSlotId = creationResult.Value.Id;

        var bookTimeSlotCommand = new BookTimeSlotsDto
        {
            TimeSlotId = timeSlotId
        };

        _mockHttpContext.SetUser(customerUser);

        // Act
        var result = await bookingController.BookTimeSlotsAsync(bookTimeSlotCommand);

        // Assert
        result.ValidateOkResult<BookingDto>(booking =>
        {
            booking.Id.Should().NotBeEmpty();
            booking.Status.Should().Be(BookingStatus.Pending);
            booking.User.Id.Should().Be(customerUser.Id);
            booking.TimeSlot.Id.Should().Be(timeSlotId);
        });

        var bookings = await _bookMeContext.Bookings
            .ToListAsync();

        bookings.Should().HaveCount(1);
    }

    [Fact]
    public async Task BookTimeSlotWithNonCustomerUser_ShouldNotSucceedAsync()
    {
        // Arrange
        _mockHttpContext.SetUser(adminUser);
        var mediator = _scope.ServiceProvider.GetRequiredService<IMediator>();
        var timeSlotQueries = _scope.ServiceProvider.GetRequiredService<ITimeSlotQueries>();
        var bookingController = new BookingController(mediator, timeSlotQueries);

         var createTimeSlotsCommand = new CreateTimeSlotCommand
       (DateTime.UtcNow.AddDays(10).AddHours(1),
             DateTime.UtcNow.AddDays(10).AddHours(2)
       );


        var creationResult = await mediator.Send(createTimeSlotsCommand);

        var timeSlotId = creationResult.Value.Id;


        var bookTimeSlotCommand = new BookTimeSlotsDto
        {
            TimeSlotId = timeSlotId
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => bookingController.BookTimeSlotsAsync(bookTimeSlotCommand));

        exception.Message.Should().Be($"Validation failed: \n -- UserDTo: User {adminUser.Id} is not a customer");

        var bookings = await _bookMeContext.Bookings
            .ToListAsync();

        bookings.Should().HaveCount(0);
    }
    #endregion
}
