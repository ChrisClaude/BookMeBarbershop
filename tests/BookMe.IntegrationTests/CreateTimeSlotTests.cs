using System.Net.Http.Json;
using BookMe.Application.Common.Bookings.Dtos;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Enums;
using BookMe.Application.Mappings;
using BookMe.IntegrationTests.TestData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BookMe.IntegrationTests;

public class CreateTimeSlotTests : BaseIntegrationTest
{
    private readonly UserDto _adminUser;
    private readonly UserDto _customerUser;

    public CreateTimeSlotTests(AspireIntegrationTestFixture factory)
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
            .First(x => x.UserRoles.Any(y => y.RoleId == DefaultRoles.CustomerId))
            .MapToDto();
    }

    #region CreateTimeSlot tests
    [Fact]
    public async Task CreateTimeSlotsShouldSucceedAsync()
    {
        // Arrange
        SetUser(_adminUser);

        var createTimeSlotsRequest = new CreateTimeSlotsDto
        {
            StartDateTime = DateTime.UtcNow.AddDays(10).AddHours(1),
            EndDateTime = DateTime.UtcNow.AddDays(10).AddHours(2),
            IsAllDay = false,
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/booking/timeslots", createTimeSlotsRequest);

        // Assert
        var timeSlots = await response.ShouldBeOkAsync<List<TimeSlotDto>>();
        var timeSlot = timeSlots.First();
        timeSlot.Start.Should().Be(createTimeSlotsRequest.StartDateTime);
        timeSlot.End.Should().Be(createTimeSlotsRequest.EndDateTime);
        timeSlot.IsAvailable.Should().BeTrue();
        timeSlot.Id.Should().NotBeEmpty();

        var persistedTimeSlots = await _bookMeContext.TimeSlots.ToListAsync();
        persistedTimeSlots.Should().HaveCount(1);

        await TestDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task CreateOverlappingTimeSlotsShouldNotSucceedAsync()
    {
        // Arrange
        SetUser(_adminUser);

        var firstCreateTimeSlotsRequest = new CreateTimeSlotsDto
        {
            StartDateTime = DateTime.UtcNow.AddDays(10).AddHours(1),
            EndDateTime = DateTime.UtcNow.AddDays(10).AddHours(2),
            IsAllDay = false,
        };

        var firstResponse = await _client.PostAsJsonAsync(
            "api/booking/timeslots",
            firstCreateTimeSlotsRequest
        );
        await firstResponse.ShouldBeOkAsync<List<TimeSlotDto>>();

        var createTimeSlotsRequest = new CreateTimeSlotsDto
        {
            StartDateTime = DateTime.UtcNow.AddDays(10).AddHours(1),
            EndDateTime = DateTime.UtcNow.AddDays(10).AddHours(2),
            IsAllDay = false,
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/booking/timeslots", createTimeSlotsRequest);

        // Assert
        var errors = await response.ShouldBeBadRequestAsync();
        errors.Should().NotBeEmpty();
        var firstError = errors.First();
        firstError.Should().NotBeNull();
        firstError.Code.Should().Be("conflict");
        firstError.Description.Should().Be("The requested time slot overlaps with existing time slots");

        var timeSlots = await _bookMeContext.TimeSlots.ToListAsync();
        timeSlots.Should().HaveCount(1);

        await TestDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task CreateTimeSlotsWithNonAdminUser_ShouldNotSucceedAsync()
    {
        // Arrange
        SetUser(_customerUser);

        var createTimeSlotsRequest = new CreateTimeSlotsDto
        {
            StartDateTime = DateTime.UtcNow.AddDays(10).AddHours(1),
            EndDateTime = DateTime.UtcNow.AddDays(10).AddHours(2),
            IsAllDay = false,
        };

        // Act
        // Policy.ADMIN rejects the customer before the controller/validator ever runs.
        var response = await _client.PostAsJsonAsync("api/booking/timeslots", createTimeSlotsRequest);

        // Assert
        await response.ShouldBeForbiddenAsync();

        var timeSlots = await _bookMeContext.TimeSlots.ToListAsync();
        timeSlots.Should().HaveCount(0);

        await TestDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(30)]
    public async Task CreateTimeSlotWithDifferentDays_ShouldSucceedAsync(int daysInFuture)
    {
        // Arrange
        SetUser(_adminUser);

        var createTimeSlotsRequest = new CreateTimeSlotsDto
        {
            StartDateTime = DateTime.UtcNow.AddDays(daysInFuture).AddHours(1),
            EndDateTime = DateTime.UtcNow.AddDays(daysInFuture).AddHours(2),
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/booking/timeslots", createTimeSlotsRequest);

        // Assert
        var timeSlots = await response.ShouldBeOkAsync<List<TimeSlotDto>>();
        timeSlots.Should().HaveCount(1);
        var timeSlot = timeSlots.First();
        timeSlot.Start.Should().Be(createTimeSlotsRequest.StartDateTime);
        timeSlot.End.Should().Be(createTimeSlotsRequest.EndDateTime);
        timeSlot.IsAvailable.Should().BeTrue();
        timeSlot.AllowAutoConfirmation.Should().BeFalse();
        timeSlot.Id.Should().NotBeEmpty();

        await TestDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(30)]
    public async Task CreateTimeSlotWithDifferentDaysAndAutoConfirmation_ShouldSucceedAsync(
        int daysInFuture
    )
    {
        // Arrange
        SetUser(_adminUser);

        var createTimeSlotsRequest = new CreateTimeSlotsDto
        {
            StartDateTime = DateTime.UtcNow.AddDays(daysInFuture).AddHours(1),
            EndDateTime = DateTime.UtcNow.AddDays(daysInFuture).AddHours(2),
            AllowAutoConfirmation = true,
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/booking/timeslots", createTimeSlotsRequest);

        // Assert
        var timeSlots = await response.ShouldBeOkAsync<List<TimeSlotDto>>();
        timeSlots.Should().HaveCount(1);
        var timeSlot = timeSlots.First();
        timeSlot.Start.Should().Be(createTimeSlotsRequest.StartDateTime);
        timeSlot.End.Should().Be(createTimeSlotsRequest.EndDateTime);
        timeSlot.IsAvailable.Should().BeTrue();
        timeSlot.Id.Should().NotBeEmpty();
        timeSlot.AllowAutoConfirmation.Should().BeTrue();

        await TestDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }
    #endregion

    #region All day time slot tests
    [Fact]
    public async Task CreateAllDayTimeSlot_ShouldSucceedAsync()
    {
        // Arrange
        SetUser(_adminUser);

        var createTimeSlotsRequest = new CreateTimeSlotsDto
        {
            StartDateTime = DateTime.Today.AddDays(1).AddHours(9),
            EndDateTime = DateTime.Today.AddDays(1).AddHours(21),
            IsAllDay = true,
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/booking/timeslots", createTimeSlotsRequest);

        // Assert
        var timeSlots = await response.ShouldBeOkAsync<List<TimeSlotDto>>();
        timeSlots.Should().HaveCount(12);
        timeSlots.Should().OnlyContain(x => x.Id != Guid.Empty);
        timeSlots.Select(x => x.Id).Should().OnlyHaveUniqueItems();

        timeSlots.Should().OnlyContain(x => x.AllowAutoConfirmation == false);

        // Verify each timeslot has 1-hour duration
        timeSlots.Should().OnlyContain(x => (x.End - x.Start) == TimeSpan.FromHours(1));

        // Verify timeslots are sequential (ordered by start time)
        var orderedTimeSlots = timeSlots.OrderBy(x => x.Start).ToList();
        for (int i = 0; i < orderedTimeSlots.Count - 1; i++)
        {
            orderedTimeSlots[i].End.Should().Be(orderedTimeSlots[i + 1].Start);
        }

        // Verify first and last timeslot match request times
        orderedTimeSlots.First().Start.Should().Be(createTimeSlotsRequest.StartDateTime);
        orderedTimeSlots.Last().End.Should().Be(createTimeSlotsRequest.EndDateTime);

        await TestDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task CreateAllDayTimeSlotWithAutoConfirmation_ShouldSucceedAsync()
    {
        // Arrange
        SetUser(_adminUser);

        var createTimeSlotsRequest = new CreateTimeSlotsDto
        {
            StartDateTime = DateTime.Today.AddDays(1),
            EndDateTime = DateTime.Today.AddDays(1).AddHours(23),
            IsAllDay = true,
            AllowAutoConfirmation = true,
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/booking/timeslots", createTimeSlotsRequest);

        // Assert
        var timeSlots = await response.ShouldBeOkAsync<List<TimeSlotDto>>();
        timeSlots.Should().OnlyContain(x => x.AllowAutoConfirmation == true);
        timeSlots.Should().HaveCount(23);
        timeSlots.Should().OnlyContain(x => x.Id != Guid.Empty);
        timeSlots.Select(x => x.Id).Should().OnlyHaveUniqueItems();

        // Verify each timeslot has 1-hour duration
        timeSlots.Should().OnlyContain(x => (x.End - x.Start) == TimeSpan.FromHours(1));

        // Verify timeslots are sequential (ordered by start time)
        var orderedTimeSlots = timeSlots.OrderBy(x => x.Start).ToList();
        for (int i = 0; i < orderedTimeSlots.Count - 1; i++)
        {
            orderedTimeSlots[i].End.Should().Be(orderedTimeSlots[i + 1].Start);
        }

        // Verify first and last timeslot match request times
        orderedTimeSlots.First().Start.Should().Be(createTimeSlotsRequest.StartDateTime);
        orderedTimeSlots.Last().End.Should().Be(createTimeSlotsRequest.EndDateTime);

        await TestDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task CreateAllDayTimeSlotWithMoreThan24Hours_ShouldNotSucceedAsync()
    {
        // Arrange
        SetUser(_adminUser);

        var createTimeSlotsRequest = new CreateTimeSlotsDto
        {
            StartDateTime = DateTime.Today.AddDays(1),
            EndDateTime = DateTime.Today.AddDays(1).AddHours(24).AddMinutes(30),
            IsAllDay = true,
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/booking/timeslots", createTimeSlotsRequest);

        // Assert
        var errors = await response.ShouldBeBadRequestAsync();
        errors
            .Should()
            .Contain(error =>
                error.Description.Contains(
                    "All-day time slots must not be apart by more than 24 hours"
                )
            );

        var timeSlots = await _bookMeContext.TimeSlots.ToListAsync();
        timeSlots.Should().HaveCount(0);

        await TestDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }
    #endregion
}