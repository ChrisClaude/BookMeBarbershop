using System.Net.Http.Json;
using BookMe.Application.Common.Bookings.Dtos;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Common.Dtos.Bookings;
using BookMe.Application.Entities;
using BookMe.Application.Enums;
using BookMe.Application.Mappings;
using BookMe.IntegrationTests.TestData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BookMe.IntegrationTests;

public class BookingTests : BaseIntegrationTest
{
    private readonly UserDto _adminUser;
    private readonly UserDto _customerUser;

    public BookingTests(AspireIntegrationTestFixture factory)
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
    }

    private async Task<Guid> CreateTimeSlotAsync()
    {
        SetUser(_adminUser);

        var response = await _client.PostAsJsonAsync(
            "api/booking/timeslots",
            new CreateTimeSlotsDto
            {
                StartDateTime = DateTime.UtcNow.AddDays(10).AddHours(1),
                EndDateTime = DateTime.UtcNow.AddDays(10).AddHours(2),
            }
        );

        var timeSlots = await response.ShouldBeOkAsync<List<TimeSlotDto>>();
        return timeSlots.First().Id;
    }

    #region BookTimeSlot tests
    [Fact]
    public async Task BookTimeSlotWithCustomerUser_ShouldSucceedAsync()
    {
        // Arrange
        var timeSlotId = await CreateTimeSlotAsync();
        SetUser(_customerUser);

        // Act
        var response = await _client.PostAsJsonAsync(
            "api/booking/book-timeslot",
            new BookTimeSlotsDto { TimeSlotId = timeSlotId }
        );

        // Assert
        var booking = await response.ShouldBeOkAsync<BookingDto>();
        booking.Id.Should().NotBeEmpty();
        booking.Status.Should().Be(BookingStatus.Pending);
        booking.User.Id.Should().Be(_customerUser.Id);
        booking.TimeSlot.Id.Should().Be(timeSlotId);

        var bookings = await _bookMeContext.Bookings.ToListAsync();
        bookings.Should().HaveCount(1);

        await TestDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
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
            Email = "jane.doe.unverified@test.com",
            PhoneNumber = "+48600697524",
            IsPhoneNumberVerified = false,
            UserRoles = new List<UserRole> { new() { RoleId = DefaultRoles.CustomerId } },
        };

        _bookMeContext.Users.Add(customerWithNonVerifiedPhoneNumber);
        await _bookMeContext.SaveChangesAsync();

        var timeSlotId = await CreateTimeSlotAsync();
        SetUser(customerWithNonVerifiedPhoneNumber.MapToDto());

        // Act
        var response = await _client.PostAsJsonAsync(
            "api/booking/book-timeslot",
            new BookTimeSlotsDto { TimeSlotId = timeSlotId }
        );

        // Assert
        var errors = await response.ShouldBeBadRequestAsync();
        errors
            .Should()
            .Contain(error =>
                error.Description.Contains(
                    $"User {customerWithNonVerifiedPhoneNumber.Id} has not verified their phone number"
                )
            );

        var bookings = await _bookMeContext.Bookings.ToListAsync();
        bookings.Should().HaveCount(0);

        await TestDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task BookTimeSlotWithNonCustomerUser_ShouldFailAsync()
    {
        // Arrange
        var timeSlotId = await CreateTimeSlotAsync();
        SetUser(_adminUser);

        // Act
        // Policy.CUSTOMER rejects the admin before the controller/validator ever runs.
        var response = await _client.PostAsJsonAsync(
            "api/booking/book-timeslot",
            new BookTimeSlotsDto { TimeSlotId = timeSlotId }
        );

        // Assert
        await response.ShouldBeForbiddenAsync();

        var bookings = await _bookMeContext.Bookings.ToListAsync();
        bookings.Should().HaveCount(0);

        await TestDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task BookTimeSlotThatIsPartOfCancelledBooking_ShouldSucceedAsync()
    {
        // Arrange
        var timeSlotId = await CreateTimeSlotAsync();

        // First booking
        SetUser(_customerUser);
        var bookResponse = await _client.PostAsJsonAsync(
            "api/booking/book-timeslot",
            new BookTimeSlotsDto { TimeSlotId = timeSlotId }
        );
        var firstBooking = await bookResponse.ShouldBeOkAsync<BookingDto>();

        // Cancel the booking
        var cancelResponse = await _client.PostAsJsonAsync(
            "api/booking/cancel-booking",
            new CancelBookingDto { BookingId = firstBooking.Id }
        );
        await cancelResponse.ShouldBeNoContentAsync();

        // Act - second booking attempt on the now-freed time slot
        var response = await _client.PostAsJsonAsync(
            "api/booking/book-timeslot",
            new BookTimeSlotsDto { TimeSlotId = timeSlotId }
        );

        // Assert
        var booking = await response.ShouldBeOkAsync<BookingDto>();
        booking.Id.Should().NotBeEmpty();
        booking.Status.Should().Be(BookingStatus.Pending);
        booking.User.Id.Should().Be(_customerUser.Id);
        booking.TimeSlot.Id.Should().Be(timeSlotId);

        var bookings = await _bookMeContext
            .Bookings.Include(book => book.TimeSlot)
            .Where(booking => booking.TimeSlotId.Equals(timeSlotId))
            .ToListAsync();

        bookings.Should().HaveCount(2);
        bookings.Where(booking => booking.Status == BookingStatus.Cancelled).Should().HaveCount(1);
        bookings.Where(booking => booking.Status == BookingStatus.Pending).Should().HaveCount(1);

        await TestDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task BookAlreadyBookedTimeSlot_ShouldFailAsync()
    {
        // Arrange
        var timeSlotId = await CreateTimeSlotAsync();

        SetUser(_customerUser);
        await _client.PostAsJsonAsync(
            "api/booking/book-timeslot",
            new BookTimeSlotsDto { TimeSlotId = timeSlotId }
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "api/booking/book-timeslot",
            new BookTimeSlotsDto { TimeSlotId = timeSlotId }
        );

        // Assert
        var errors = await response.ShouldBeBadRequestAsync();
        errors
            .Should()
            .Contain(error =>
                error.Description.Contains($"Time slot with id {timeSlotId} is not available")
            );

        var bookings = await _bookMeContext.Bookings.ToListAsync();
        bookings.Should().HaveCount(1);

        await TestDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task BookTimeSlotWithInvalidId_ShouldFailAsync()
    {
        // Arrange
        var timeSlotId = Guid.NewGuid();
        SetUser(_customerUser);

        // Act
        var response = await _client.PostAsJsonAsync(
            "api/booking/book-timeslot",
            new BookTimeSlotsDto { TimeSlotId = timeSlotId }
        );

        // Assert
        var errors = await response.ShouldBeBadRequestAsync();
        errors
            .Should()
            .Contain(error =>
                error.Description.Contains(
                    $"Time slot with id {timeSlotId} not found by user {_customerUser.Id}"
                )
            );

        var bookings = await _bookMeContext.Bookings.ToListAsync();
        bookings.Should().HaveCount(0);

        await TestDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }
    #endregion

    #region Booking cancellation
    [Fact]
    public async Task CancelBookingShouldSucceedAsync()
    {
        // Arrange - create a booking first
        var timeSlotId = await CreateTimeSlotAsync();

        SetUser(_customerUser);
        var bookResponse = await _client.PostAsJsonAsync(
            "api/booking/book-timeslot",
            new BookTimeSlotsDto { TimeSlotId = timeSlotId }
        );
        var booking = await bookResponse.ShouldBeOkAsync<BookingDto>();

        // Act
        var cancelResponse = await _client.PostAsJsonAsync(
            "api/booking/cancel-booking",
            new CancelBookingDto { BookingId = booking.Id }
        );

        // Assert
        await cancelResponse.ShouldBeNoContentAsync();

        var persistedBooking = await _bookMeContext.Bookings.FirstAsync(b => b.Id == booking.Id);
        persistedBooking.Status.Should().Be(BookingStatus.Cancelled);

        var timeSlot = await _bookMeContext.TimeSlots.FirstAsync(ts => ts.Id == timeSlotId);
        timeSlot.IsAvailable.Should().BeTrue();

        await TestDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }
    #endregion

    #region Booking confirmation
    [Fact]
    public async Task ConfirmBookingShouldSucceedAsync()
    {
        // Arrange
        var timeSlotId = await CreateTimeSlotAsync();

        SetUser(_customerUser);
        var bookResponse = await _client.PostAsJsonAsync(
            "api/booking/book-timeslot",
            new BookTimeSlotsDto { TimeSlotId = timeSlotId }
        );
        var booking = await bookResponse.ShouldBeOkAsync<BookingDto>();

        // Act
        SetUser(_adminUser);
        var confirmResponse = await _client.PostAsJsonAsync(
            "api/booking/confirm",
            new ConfirmBookingDto { BookingId = booking.Id }
        );

        // Assert
        var confirmedBooking = await confirmResponse.ShouldBeOkAsync<BookingDto>();
        confirmedBooking.Id.Should().Be(booking.Id);
        confirmedBooking.Status.Should().Be(BookingStatus.Confirmed);
        confirmedBooking.TimeSlot.Id.Should().Be(timeSlotId);

        var persistedBooking = await _bookMeContext.Bookings.FirstAsync(b => b.Id == booking.Id);
        persistedBooking.Status.Should().Be(BookingStatus.Confirmed);

        await TestDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task ConfirmBookingWithNonAdminUser_ShouldFailAsync()
    {
        // Arrange
        var timeSlotId = await CreateTimeSlotAsync();

        SetUser(_customerUser);
        var bookResponse = await _client.PostAsJsonAsync(
            "api/booking/book-timeslot",
            new BookTimeSlotsDto { TimeSlotId = timeSlotId }
        );
        var booking = await bookResponse.ShouldBeOkAsync<BookingDto>();

        // Act
        // Policy.ADMIN rejects the customer before the controller/validator ever runs.
        var confirmResponse = await _client.PostAsJsonAsync(
            "api/booking/confirm",
            new ConfirmBookingDto { BookingId = booking.Id }
        );

        // Assert
        await confirmResponse.ShouldBeForbiddenAsync();

        var persistedBooking = await _bookMeContext.Bookings.FirstAsync(b => b.Id == booking.Id);
        persistedBooking.Status.Should().Be(BookingStatus.Pending);

        await TestDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task ConfirmNonExistentBooking_ShouldFailAsync()
    {
        // Arrange
        var nonExistentBookingId = Guid.NewGuid();
        SetUser(_adminUser);

        // Act
        var response = await _client.PostAsJsonAsync(
            "api/booking/confirm",
            new ConfirmBookingDto { BookingId = nonExistentBookingId }
        );

        // Assert
        var errors = await response.ShouldBeBadRequestAsync();
        errors
            .Should()
            .Contain(error =>
                error.Description.Contains($"Booking with id {nonExistentBookingId} not found")
            );

        await TestDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task GetUsersBookings_ShouldSucceedAsync()
    {
        // Arrange
        var timeSlotId = await CreateTimeSlotAsync();

        SetUser(_customerUser);
        await _client.PostAsJsonAsync(
            "api/booking/book-timeslot",
            new BookTimeSlotsDto { TimeSlotId = timeSlotId }
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            "api/booking/get-bookings",
            new GetBookingsDto { FromDateTime = DateTime.Today }
        );

        // Assert
        var bookings = await response.ShouldBeOkAsync<PagedListDto<BookingDto>>();
        bookings.Items.Should().HaveCount(1);
        bookings.Items.First().Status.Should().Be(BookingStatus.Pending);
        bookings.Items.First().User.Id.Should().Be(_customerUser.Id);
        bookings.Items.First().TimeSlot.Id.Should().Be(timeSlotId);

        await TestDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task GetAllBookings_ShouldSucceedAsync()
    {
        // Arrange
        var timeSlotId = await CreateTimeSlotAsync();

        SetUser(_customerUser);
        await _client.PostAsJsonAsync(
            "api/booking/book-timeslot",
            new BookTimeSlotsDto { TimeSlotId = timeSlotId }
        );

        // Act
        SetUser(_adminUser);
        var response = await _client.PostAsJsonAsync(
            "api/booking/get-bookings/all",
            new GetBookingsDto { FromDateTime = DateTime.Today }
        );

        // Assert
        var bookings = await response.ShouldBeOkAsync<PagedListDto<BookingDto>>();
        bookings.Items.Should().HaveCount(1);
        bookings.Items.First().Status.Should().Be(BookingStatus.Pending);
        bookings.Items.First().User.Id.Should().Be(_customerUser.Id);
        bookings.Items.First().TimeSlot.Id.Should().Be(timeSlotId);

        await TestDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }
    #endregion
}