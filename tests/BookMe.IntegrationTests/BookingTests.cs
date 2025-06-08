using BookMe.Application.Commands.Bookings;
using BookMe.Application.Common;
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
    private UserDto _adminUser;
    private UserDto _customerUser;
    private BookingController _bookingController;
    private IMediator _mediator;
    private ITimeSlotQueries _timeSlotQueries;

    private IBookingQueries _bookingService;

    public BookingTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
        _adminUser = _bookMeContext
            .Users.Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .First(x => x.UserRoles.Any(y => y.RoleId == DefaultRoles.AdminId))
            .MapToDto();

        _customerUser = _bookMeContext
            .Users.Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .First(x => x.Email == "jane.doe.customer@test.com")
            .MapToDto();

        _mediator = _scope.ServiceProvider.GetRequiredService<IMediator>();
        _timeSlotQueries = _scope.ServiceProvider.GetRequiredService<ITimeSlotQueries>();
        _bookingService = _scope.ServiceProvider.GetRequiredService<IBookingQueries>();
        _bookingController = new BookingController(_mediator, _timeSlotQueries, _bookingService);

        _bookingController.ControllerContext = new ControllerContext
        {
            HttpContext = _mockHttpContext.HttpContext!,
        };
    }

    #region BookTimeSlot tests
    [Fact]
    public async Task BookTimeSlotWithCustomerUser_ShouldSucceedAsync()
    {
        // Arrange
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

        await TestCDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task BookTimeSlotWithNonVerifiedPhoneNumber_ShouldFailAsync()
    {
        // Arrange
        var customerWithNonVerifiedPhoneNumber = new User
        {
            Id = Guid.NewGuid(),
            Name = "Jane",
            Surname = "Doe",
            Email = "jane.doe.customer@test.com",
            PhoneNumber = "+48600697524",
            IsPhoneNumberVerified = false,
            UserRoles = new List<UserRole> { new() { RoleId = DefaultRoles.CustomerId } },
        };

        _bookMeContext.Users.Add(customerWithNonVerifiedPhoneNumber);
        await _bookMeContext.SaveChangesAsync();

        var createTimeSlotsCommand = new CreateTimeSlotCommand(
            DateTime.UtcNow.AddDays(10).AddHours(1),
            DateTime.UtcNow.AddDays(10).AddHours(2)
        );

        _mockHttpContext.SetUser(_adminUser);

        var creationResult = await _mediator.Send(createTimeSlotsCommand);

        var timeSlotId = creationResult.Value.First().Id;

        var bookTimeSlotCommand = new BookTimeSlotsDto { TimeSlotId = timeSlotId };

        _mockHttpContext.SetUser(customerWithNonVerifiedPhoneNumber.MapToDto());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _bookingController.BookTimeSlotsAsync(bookTimeSlotCommand)
        );

        exception
            .Message.Should()
            .Be(
                $"Validation failed: \n -- UserDto: User {customerWithNonVerifiedPhoneNumber.Id} has not verified their phone number"
            );

        var bookings = await _bookMeContext.Bookings.ToListAsync();
        bookings.Should().HaveCount(0);

        await TestCDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task BookTimeSlotWithNonCustomerUser_ShouldFailAsync()
    {
        // Arrange
        _mockHttpContext.SetUser(_adminUser);

        var createTimeSlotsCommand = new CreateTimeSlotCommand(
            DateTime.UtcNow.AddDays(10).AddHours(1),
            DateTime.UtcNow.AddDays(10).AddHours(2)
        );

        var creationResult = await _mediator.Send(createTimeSlotsCommand);

        var timeSlotId = creationResult.Value.First().Id;

        var bookTimeSlotCommand = new BookTimeSlotsDto { TimeSlotId = timeSlotId };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _bookingController.BookTimeSlotsAsync(bookTimeSlotCommand)
        );

        exception
            .Message.Should()
            .Be($"Validation failed: \n -- UserDto: User {_adminUser.Id} is not a customer");

        var bookings = await _bookMeContext.Bookings.ToListAsync();

        bookings.Should().HaveCount(0);
        await TestCDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task BookTimeSlotThatIsPartOfCancelledBooking_ShouldSucceedAsync()
    {
        // Arrange

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
        var bookingId = ((OkObjectResult)bookResult)
            .Value.GetType()
            .GetProperty("Id")
            .GetValue(((OkObjectResult)bookResult).Value);
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

        var bookings = await _bookMeContext
            .Bookings.Include(book => book.TimeSlot)
            .Where(booking => booking.TimeSlotId.Equals(timeSlotId))
            .ToListAsync();

        bookings.Should().HaveCount(2);
        bookings.Where(booking => booking.Status == BookingStatus.Cancelled).Should().HaveCount(1);
        bookings.Where(booking => booking.Status == BookingStatus.Pending).Should().HaveCount(1);

        await TestCDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task BookAlreadyBookedTimeSlot_ShouldFailAsync()
    {
        // Arrange

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
                .Any(error =>
                    error.Description.Contains($"Time slot with id {timeSlotId} is not available")
                )
                .Should()
                .BeTrue();
        });

        var bookings = await _bookMeContext.Bookings.ToListAsync();
        bookings.Should().HaveCount(1);

        await TestCDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task BookTimeSlotWithInvalidId_ShouldFailAsync()
    {
        // Arrange
        var timeSlotId = Guid.NewGuid();
        _mockHttpContext.SetUser(_customerUser);
        var bookTimeSlotCommand = new BookTimeSlotsDto
        {
            TimeSlotId = timeSlotId, // Non-existent ID
        };

        // Act
        var result = await _bookingController.BookTimeSlotsAsync(bookTimeSlotCommand);

        // Assert
        result.ValidateBadRequestResult<List<Error>>(errors =>
        {
            errors.Should().HaveCount(1);
            errors
                .Any(error =>
                    error.Description.Contains(
                        $"Time slot with id {timeSlotId} not found by user {_customerUser.Id}"
                    )
                )
                .Should()
                .BeTrue();
        });

        var bookings = await _bookMeContext.Bookings.ToListAsync();
        bookings.Should().HaveCount(0);

        await TestCDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }
    #endregion

    #region Booking cancellation
    [Fact]
    public async Task CancelBookingShouldSucceedAsync()
    {
        // Arrange - Create a booking first

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
        var bookingId = ((OkObjectResult)bookResult)
            .Value.GetType()
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
        await TestCDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }
    #endregion

    #region Booking confirmation
    [Fact]
    public async Task ConfirmBookingShouldSucceedAsync()
    {
        // Arrange

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
        var bookingId = ((OkObjectResult)bookResult)
            .Value.GetType()
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
        await TestCDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task ConfirmBookingWithNonAdminUser_ShouldFailAsync()
    {
        // Arrange

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
        var bookingId = ((OkObjectResult)bookResult)
            .Value.GetType()
            .GetProperty("Id")
            .GetValue(((OkObjectResult)bookResult).Value);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        // Act & Assert - Try to confirm as customer
#pragma warning disable CS8605 // Unboxing a possibly null value.
        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _bookingController.ConfirmBookingAsync(
                new ConfirmBookingDto { BookingId = (Guid)bookingId }
            )
        );
#pragma warning restore CS8605 // Unboxing a possibly null value.

        exception
            .Message.Should()
            .Be($"Validation failed: \n -- UserDto: User {_customerUser.Id} is not an admin");

#pragma warning disable CS8605 // Unboxing a possibly null value.
        var booking = await _bookMeContext.Bookings.FirstAsync(b => b.Id == (Guid)bookingId);
#pragma warning restore CS8605 // Unboxing a possibly null value.

        booking.Status.Should().Be(BookingStatus.Pending);

        await TestCDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task ConfirmNonExistentBooking_ShouldFailAsync()
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
                .Any(error =>
                    error.Description.Contains($"Booking with id {nonExistentBookingId} not found")
                )
                .Should()
                .BeTrue();
        });

        await TestCDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task GetUsersBookings_ShouldSucceedAsync()
    {
        // Arrange

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

        var request = new GetBookingsDto { FromDateTime = DateTime.Today };

        // Act
        var result = await _bookingController.GetUsersBookingsAsync(request);

        // Assert
        result.ValidateOkResult<PagedListDto<BookingDto>>(bookings =>
        {
            bookings.Items.Should().HaveCount(1);
            bookings.Items.First().Status.Should().Be(BookingStatus.Pending);
            bookings.Items.First().User.Id.Should().Be(_customerUser.Id);
            bookings.Items.First().TimeSlot.Id.Should().Be(timeSlotId);
        });

        await TestCDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task GetAllBookings_ShouldSucceedAsync()
    {
        // Arrange

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

        _mockHttpContext.SetUser(_adminUser);
        var request = new GetBookingsDto { FromDateTime = DateTime.Today };

        // Act
        var result = await _bookingController.GetAllBookingsAsync(request);

        // Assert
        result.ValidateOkResult<PagedListDto<BookingDto>>(bookings =>
        {
            bookings.Items.Should().HaveCount(1);
            bookings.Items.First().Status.Should().Be(BookingStatus.Pending);
            bookings.Items.First().User.Id.Should().Be(_customerUser.Id);
            bookings.Items.First().TimeSlot.Id.Should().Be(timeSlotId);
        });

        await TestCDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    #endregion
}
