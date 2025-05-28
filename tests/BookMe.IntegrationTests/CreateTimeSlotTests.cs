using BookMe.Application.Common;
using BookMe.Application.Common.Bookings.Dtos;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Enums;
using BookMe.Application.Interfaces.Queries;
using BookMe.Application.Mappings;
using BookMeAPI.Controllers;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookMe.IntegrationTests;

public class CreateTimeSlotTests : BaseIntegrationTest
{
    private UserDto _adminUser;
    private UserDto _customerUser;
    private BookingController _bookingController;
    private IMediator _mediator;
    private ITimeSlotQueries _timeSlotQueries;

    public CreateTimeSlotTests(IntegrationTestWebAppFactory factory)
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

    [Fact]
    public async Task CreateAllDayTimeSlot_ShouldSucceedAsync()
    {
        // Arrange
        await _bookMeContext.TimeSlots.ExecuteDeleteAsync();
        _mockHttpContext.SetUser(_adminUser);

        var createTimeSlotsRequest = new CreateTimeSlotsDto
        {
            StartDateTime = DateTime.Today.AddDays(1),
            EndDateTime = DateTime.Today.AddDays(1).AddHours(23),
            IsAllDay = true,
        };

        // Act
        var result = await _bookingController.CreateTimeSlotsAsync(createTimeSlotsRequest);

        // Assert
        result.ValidateOkResult<List<TimeSlotDto>>(timeSlots =>
        {
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
        });
    }
    #endregion
}
