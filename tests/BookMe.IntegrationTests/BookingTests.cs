using BookMe.Application.Commands.Bookings;
using BookMe.Application.Common;
using BookMe.Application.Common.Bookings.Dtos;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Common.Dtos.Bookings;
using BookMe.Application.Entities;
using BookMe.Application.Enums;
using BookMe.Application.Interfaces.Queries;
using BookMe.Application.Mappings;
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
    private UserDto _adminUser;
    private UserDto _customerUser;
    private BookingController _bookingController;
    private IMediator _mediator;
    private ITimeSlotQueries _timeSlotQueries;

    public BookingTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
        _adminUser = _bookMeContext.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .First(x => x.UserRoles.Any(y => y.RoleId == DefaultRoles.AdminId))
            .MapToDto();

        _customerUser = _bookMeContext.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .First(x => x.UserRoles.Any(y => y.RoleId == DefaultRoles.CustomerId))
            .MapToDto();

        _mediator = _scope.ServiceProvider.GetRequiredService<IMediator>();
        _timeSlotQueries = _scope.ServiceProvider.GetRequiredService<ITimeSlotQueries>();
        _bookingController = new BookingController(_mediator, _timeSlotQueries);
    }

    #region CreateTimeSlot tests
    [Fact]
    public async Task CreateTimeSlotsShouldSucceedAsync()
    {
        // Arrange
        await _bookMeContext.TimeSlots.ExecuteDeleteAsync();
        _mockHttpContext.SetUser(_adminUser);

        var createTimeSlotsRequest = new CreateTimeSlotsDto
        {
            StartDateTime = DateTime.UtcNow.AddDays(10).AddHours(1),
            EndDateTime = DateTime.UtcNow.AddDays(10).AddHours(2),
            IsAllDay = false,
        };

        // Act
        var result = await _bookingController.CreateTimeSlotsAsync(createTimeSlotsRequest);

        // Assert
        result.ValidateOkResult<List<TimeSlotDto>>(timeSlots =>
        {
            var timeSlot = timeSlots.First();
            timeSlot.Start.Should().Be(createTimeSlotsRequest.StartDateTime);
            timeSlot.End.Should().Be(createTimeSlotsRequest.EndDateTime);
            timeSlot.IsAvailable.Should().BeTrue();
            timeSlot.Id.Should().NotBeEmpty();
        });

        var timeSlots = await _bookMeContext.TimeSlots.ToListAsync();

        timeSlots.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateOverlappingTimeSlotsShouldNotSucceedAsync()
    {
        // Arrange
        await _bookMeContext.TimeSlots.ExecuteDeleteAsync();
        _mockHttpContext.SetUser(_adminUser);

        var firstCreateTimeSlotsRequest = new CreateTimeSlotsDto
        {
            StartDateTime = DateTime.UtcNow.AddDays(10).AddHours(1),
            EndDateTime = DateTime.UtcNow.AddDays(10).AddHours(2),
            IsAllDay = false,
        };

        var firstResult = await _bookingController.CreateTimeSlotsAsync(
            firstCreateTimeSlotsRequest
        );

        var createTimeSlotsRequest = new CreateTimeSlotsDto
        {
            StartDateTime = DateTime.UtcNow.AddDays(10).AddHours(1),
            EndDateTime = DateTime.UtcNow.AddDays(10).AddHours(2),
            IsAllDay = false,
        };

        // Act
        var result = await _bookingController.CreateTimeSlotsAsync(createTimeSlotsRequest);

        // Assert
        result.ValidateBadRequestResult<List<Error>>(errors =>
        {
            errors.Should().NotBeEmpty();
            var firstError = errors.First();
            firstError.Should().NotBeNull();
            firstError.Code.Should().Be("conflict");
            firstError.Description
                .Should()
                .Be("The requested time slot overlaps with existing time slots");
        });

        var timeSlots = await _bookMeContext.TimeSlots.ToListAsync();

        timeSlots.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateTimeSlotsWithNonAdminUser_ShouldNotSucceedAsync()
    {
        // Arrange
        await _bookMeContext.TimeSlots.ExecuteDeleteAsync();
        var customerUser = await _bookMeContext.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstAsync(x => x.UserRoles.Any(y => y.RoleId == DefaultRoles.CustomerId));

        _mockHttpContext.SetUser(customerUser.MapToDto());

        var createTimeSlotsRequest = new CreateTimeSlotsDto
        {
            StartDateTime = DateTime.UtcNow.AddDays(10).AddHours(1),
            EndDateTime = DateTime.UtcNow.AddDays(10).AddHours(2),
            IsAllDay = false,
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => _bookingController.CreateTimeSlotsAsync(createTimeSlotsRequest)
        );

        exception.Message
            .Should()
            .Be($"Validation failed: \n -- UserDto: User {customerUser.Id} is not an admin");

        var timeSlots = await _bookMeContext.TimeSlots.ToListAsync();

        timeSlots.Should().HaveCount(0);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(30)]
    public async Task CreateTimeSlotWithDifferentDays_ShouldSucceedAsync(int daysInFuture)
    {
        // Arrange
        await _bookMeContext.TimeSlots.ExecuteDeleteAsync();
        _mockHttpContext.SetUser(_adminUser);

        var createTimeSlotsRequest = new CreateTimeSlotsDto
        {
            StartDateTime = DateTime.UtcNow.AddDays(daysInFuture).AddHours(1),
            EndDateTime = DateTime.UtcNow.AddDays(daysInFuture).AddHours(2),
        };

        // Act
        var result = await _bookingController.CreateTimeSlotsAsync(createTimeSlotsRequest);

        // Assert
        result.ValidateOkResult<List<TimeSlotDto>>(timeSlots =>
        {
            timeSlots.Should().HaveCount(1);
            var timeSlot = timeSlots.First();
            timeSlot.Start.Should().Be(createTimeSlotsRequest.StartDateTime);
            timeSlot.End.Should().Be(createTimeSlotsRequest.EndDateTime);
        });
    }
    #endregion

    #region BookTimeSlot tests
    [Fact]
    public async Task BookTimeSlotShouldWithCustomerUser_ShouldSucceedAsync()
    {
        // Arrange
        await _bookMeContext.Bookings.ExecuteDeleteAsync();
        await _bookMeContext.TimeSlots.ExecuteDeleteAsync();

        _mockHttpContext.SetUser(_adminUser);

        var createTimeSlotsCommand = new CreateTimeSlotCommand(
            DateTime.UtcNow.AddDays(10).AddHours(1),
            DateTime.UtcNow.AddDays(10).AddHours(2)
        );

        var creationResult = await _mediator.Send(createTimeSlotsCommand);

        var timeSlotId = creationResult.Value.First().Id;

        var bookTimeSlotCommand = new BookTimeSlotsDto { TimeSlotId = timeSlotId };

        _mockHttpContext.SetUser(_customerUser);

        // Act
        var result = await _bookingController.BookTimeSlotsAsync(bookTimeSlotCommand);

        // Assert
        result.ValidateOkResult<BookingDto>(booking =>
        {
            booking.Id.Should().NotBeEmpty();
            booking.Status.Should().Be(BookingStatus.Pending);
            booking.User.Id.Should().Be(_customerUser.Id);
            booking.TimeSlot.Id.Should().Be(timeSlotId);
        });

        var bookings = await _bookMeContext.Bookings.ToListAsync();

        bookings.Should().HaveCount(1);
    }

    [Fact]
    public async Task BookTimeSlotWithNonCustomerUser_ShouldNotSucceedAsync()
    {
        // Arrange
        _mockHttpContext.SetUser(_adminUser);
        await _bookMeContext.TimeSlots.ExecuteDeleteAsync();

        var createTimeSlotsCommand = new CreateTimeSlotCommand(
            DateTime.UtcNow.AddDays(10).AddHours(1),
            DateTime.UtcNow.AddDays(10).AddHours(2)
        );

        var creationResult = await _mediator.Send(createTimeSlotsCommand);

        var timeSlotId = creationResult.Value.First().Id;

        var bookTimeSlotCommand = new BookTimeSlotsDto { TimeSlotId = timeSlotId };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => _bookingController.BookTimeSlotsAsync(bookTimeSlotCommand)
        );

        exception.Message
            .Should()
            .Be($"Validation failed: \n -- UserDto: User {_adminUser.Id} is not a customer");

        var bookings = await _bookMeContext.Bookings.ToListAsync();

        bookings.Should().HaveCount(0);
    }

    [Fact]
    public async Task BookTimeSlotThatIsPartOfCancelledBooking_ShouldSucceedAsync()
    {
        // Arrange
        await _bookMeContext.Bookings.ExecuteDeleteAsync();
        await _bookMeContext.TimeSlots.ExecuteDeleteAsync();

        _mockHttpContext.SetUser(_adminUser);
        var createTimeSlotsCommand = new CreateTimeSlotCommand(
            DateTime.UtcNow.AddDays(10).AddHours(1),
            DateTime.UtcNow.AddDays(10).AddHours(2)
        );
        var creationResult = await _mediator.Send(createTimeSlotsCommand);
        var timeSlotId = creationResult.Value.First().Id;

        // First booking
        _mockHttpContext.SetUser(_customerUser);
        var bookResult = await _bookingController.BookTimeSlotsAsync(
            new BookTimeSlotsDto { TimeSlotId = timeSlotId }
        );
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var bookingId = ((OkObjectResult)bookResult).Value
            .GetType()
            .GetProperty("Id")
            .GetValue(((OkObjectResult)bookResult).Value);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        // Cancel the booking
#pragma warning disable CS8605 // Unboxing a possibly null value.
        var cancelResult = await _bookingController.CancelBookingAsync(
            new CancelBookingDto { BookingId = (Guid)bookingId }
        );
#pragma warning restore CS8605 // Unboxing a possibly null value.

        // Second booking attempt
        var bookTimeSlotRequest = new BookTimeSlotsDto { TimeSlotId = timeSlotId };

        // Act & Assert
        var result = await _bookingController.BookTimeSlotsAsync(bookTimeSlotRequest);

        result.ValidateOkResult<BookingDto>(booking =>
        {
            booking.Id.Should().NotBeEmpty();
            booking.Status.Should().Be(BookingStatus.Pending);
            booking.User.Id.Should().Be(_customerUser.Id);
            booking.TimeSlot.Id.Should().Be(timeSlotId);
        });

        var bookings = await _bookMeContext.Bookings
            .Include(book => book.TimeSlot)
            .Where(booking => booking.TimeSlotId.Equals(timeSlotId))
            .ToListAsync();

        bookings.Should().HaveCount(2);
        bookings.Where(booking => booking.Status == BookingStatus.Cancelled).Should().HaveCount(1);
        bookings.Where(booking => booking.Status == BookingStatus.Pending).Should().HaveCount(1);
    }

    [Fact]
    public async Task BookAlreadyBookedTimeSlot_ShouldNotSucceedAsync()
    {
        // Arrange
        await _bookMeContext.Bookings.ExecuteDeleteAsync();
        await _bookMeContext.TimeSlots.ExecuteDeleteAsync();

        _mockHttpContext.SetUser(_adminUser);
        var createTimeSlotsCommand = new CreateTimeSlotCommand(
            DateTime.UtcNow.AddDays(10).AddHours(1),
            DateTime.UtcNow.AddDays(10).AddHours(2)
        );
        var creationResult = await _mediator.Send(createTimeSlotsCommand);
        var timeSlotId = creationResult.Value.First().Id;

        // First booking
        _mockHttpContext.SetUser(_customerUser);
        await _bookingController.BookTimeSlotsAsync(
            new BookTimeSlotsDto { TimeSlotId = timeSlotId }
        );

        // Second booking attempt
        var bookTimeSlotCommand = new BookTimeSlotsDto { TimeSlotId = timeSlotId };

        // Act & Assert
        var result = await _bookingController.BookTimeSlotsAsync(bookTimeSlotCommand);

        result.ValidateBadRequestResult<List<Error>>(errors =>
        {
            errors.Should().HaveCount(1);
            errors
                .Any(
                    error =>
                        error.Description.Contains(
                            $"Time slot with id {timeSlotId} is not available"
                        )
                )
                .Should()
                .BeTrue();
        });

        var bookings = await _bookMeContext.Bookings.ToListAsync();
        bookings.Should().HaveCount(1);
    }

    [Fact]
    public async Task BookTimeSlotWithInvalidId_ShouldNotSucceedAsync()
    {
        // Arrange
        var timeSlotId = Guid.NewGuid();
        _mockHttpContext.SetUser(_customerUser);
        var bookTimeSlotCommand = new BookTimeSlotsDto
        {
            TimeSlotId = timeSlotId // Non-existent ID
        };

        // Act
        var result = await _bookingController.BookTimeSlotsAsync(bookTimeSlotCommand);

        // Assert
        result.ValidateBadRequestResult<List<Error>>(errors =>
        {
            errors.Should().HaveCount(1);
            errors
                .Any(
                    error =>
                        error.Description.Contains(
                            $"Time slot with id {timeSlotId} not found by user {_customerUser.Id}"
                        )
                )
                .Should()
                .BeTrue();
        });

        var bookings = await _bookMeContext.Bookings.ToListAsync();
        bookings.Should().HaveCount(0);
    }
    #endregion

    #region Booking cancellation
    [Fact]
    public async Task CancelBookingShouldSucceedAsync()
    {
        // Arrange - Create a booking first
        await _bookMeContext.Bookings.ExecuteDeleteAsync();
        await _bookMeContext.TimeSlots.ExecuteDeleteAsync();

        _mockHttpContext.SetUser(_adminUser);
        var createTimeSlotsCommand = new CreateTimeSlotCommand(
            DateTime.UtcNow.AddDays(10).AddHours(1),
            DateTime.UtcNow.AddDays(10).AddHours(2)
        );
        var creationResult = await _mediator.Send(createTimeSlotsCommand);
        var timeSlotId = creationResult.Value.First().Id;

        _mockHttpContext.SetUser(_customerUser);
        var bookResult = await _bookingController.BookTimeSlotsAsync(
            new BookTimeSlotsDto { TimeSlotId = timeSlotId }
        );
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var bookingId = ((OkObjectResult)bookResult).Value
            .GetType()
            .GetProperty("Id")
            .GetValue(((OkObjectResult)bookResult).Value);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        // Act
#pragma warning disable CS8605 // Unboxing a possibly null value.
        var cancelResult = await _bookingController.CancelBookingAsync(
            new CancelBookingDto { BookingId = (Guid)bookingId }
        );
#pragma warning restore CS8605 // Unboxing a possibly null value.

        // Assert
        cancelResult.ValidateNoContentResult();

        var booking = await _bookMeContext.Bookings.FirstAsync(b => b.Id == (Guid)bookingId);

        booking.Status.Should().Be(BookingStatus.Cancelled);

        var timeSlot = await _bookMeContext.TimeSlots.FirstAsync(ts => ts.Id == timeSlotId);

        timeSlot.IsAvailable.Should().BeTrue();
    }
    #endregion

    #region Booking confirmation
    [Fact]
    public async Task ConfirmBookingShouldSucceedAsync()
    {
        // Arrange - Create a booking first
        await _bookMeContext.Bookings.ExecuteDeleteAsync();
        await _bookMeContext.TimeSlots.ExecuteDeleteAsync();

        // Create time slot as admin
        _mockHttpContext.SetUser(_adminUser);
        var createTimeSlotsCommand = new CreateTimeSlotCommand(
            DateTime.UtcNow.AddDays(10).AddHours(1),
            DateTime.UtcNow.AddDays(10).AddHours(2)
        );
        var creationResult = await _mediator.Send(createTimeSlotsCommand);
        var timeSlotId = creationResult.Value.First().Id;

        // Book as customer
        _mockHttpContext.SetUser(_customerUser);
        var bookResult = await _bookingController.BookTimeSlotsAsync(
            new BookTimeSlotsDto { TimeSlotId = timeSlotId }
        );
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var bookingId = ((OkObjectResult)bookResult).Value
            .GetType()
            .GetProperty("Id")
            .GetValue(((OkObjectResult)bookResult).Value);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        // Act - Confirm as admin
        _mockHttpContext.SetUser(_adminUser);
#pragma warning disable CS8605 // Unboxing a possibly null value.
        var confirmResult = await _bookingController.ConfirmBookingAsync(
            new ConfirmBookingDto { BookingId = (Guid)bookingId }
        );
#pragma warning restore CS8605 // Unboxing a possibly null value.

        // Assert
        confirmResult.ValidateOkResult<BookingDto>(booking =>
        {
            booking.Id.Should().Be((Guid)bookingId);
            booking.Status.Should().Be(BookingStatus.Confirmed);
            booking.TimeSlot.Id.Should().Be(timeSlotId);
        });

        var booking = await _bookMeContext.Bookings.FirstAsync(b => b.Id == (Guid)bookingId);

        booking.Status.Should().Be(BookingStatus.Confirmed);
    }

    [Fact]
    public async Task ConfirmBookingWithNonAdminUser_ShouldNotSucceedAsync()
    {
        // Arrange - Create a booking first
        await _bookMeContext.Bookings.ExecuteDeleteAsync();
        await _bookMeContext.TimeSlots.ExecuteDeleteAsync();

        // Create time slot as admin
        _mockHttpContext.SetUser(_adminUser);
        var createTimeSlotsCommand = new CreateTimeSlotCommand(
            DateTime.UtcNow.AddDays(10).AddHours(1),
            DateTime.UtcNow.AddDays(10).AddHours(2)
        );
        var creationResult = await _mediator.Send(createTimeSlotsCommand);
        var timeSlotId = creationResult.Value.First().Id;

        // Book as customer
        _mockHttpContext.SetUser(_customerUser);
        var bookResult = await _bookingController.BookTimeSlotsAsync(
            new BookTimeSlotsDto { TimeSlotId = timeSlotId }
        );
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var bookingId = ((OkObjectResult)bookResult).Value
            .GetType()
            .GetProperty("Id")
            .GetValue(((OkObjectResult)bookResult).Value);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        // Act & Assert - Try to confirm as customer
#pragma warning disable CS8605 // Unboxing a possibly null value.
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () =>
                _bookingController.ConfirmBookingAsync(
                    new ConfirmBookingDto { BookingId = (Guid)bookingId }
                )
        );
#pragma warning restore CS8605 // Unboxing a possibly null value.

        exception.Message
            .Should()
            .Be($"Validation failed: \n -- UserDto: User {_customerUser.Id} is not an admin");

#pragma warning disable CS8605 // Unboxing a possibly null value.
        var booking = await _bookMeContext.Bookings.FirstAsync(b => b.Id == (Guid)bookingId);
#pragma warning restore CS8605 // Unboxing a possibly null value.

        booking.Status.Should().Be(BookingStatus.Pending);
    }

    [Fact]
    public async Task ConfirmNonExistentBooking_ShouldNotSucceedAsync()
    {
        // Arrange
        var nonExistentBookingId = Guid.NewGuid();
        _mockHttpContext.SetUser(_adminUser);

        // Act
        var result = await _bookingController.ConfirmBookingAsync(
            new ConfirmBookingDto { BookingId = nonExistentBookingId }
        );

        // Assert
        result.ValidateBadRequestResult<List<Error>>(errors =>
        {
            errors.Should().HaveCount(1);
            errors
                .Any(
                    error =>
                        error.Description.Contains(
                            $"Booking with id {nonExistentBookingId} not found"
                        )
                )
                .Should()
                .BeTrue();
        });
    }
    #endregion
}
