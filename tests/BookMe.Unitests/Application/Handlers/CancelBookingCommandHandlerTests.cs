using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BookMe.Application.Commands.Bookings;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Common.Dtos.Users;
using BookMe.Application.Common.Errors;
using BookMe.Application.Entities;
using BookMe.Application.Enums;
using BookMe.Application.Handlers;
using BookMe.Application.Interfaces;
using FluentAssertions;
using Moq;

namespace BookMe.UnitTests.Application.Handlers;

public class CancelBookingCommandHandlerTests
{
    private readonly Mock<IRepository<Booking>> _mockBookingRepository;
    private readonly CancelBookingCommandHandler _handler;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _bookingId = Guid.NewGuid();

    public CancelBookingCommandHandlerTests()
    {
        _mockBookingRepository = new Mock<IRepository<Booking>>();
        _handler = new CancelBookingCommandHandler(_mockBookingRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenBookingExists_AndUserIsAuthorized_ShouldCancelBookingAsync()
    {
        // Arrange
        var user = new User { Id = _userId };
        var booking = new Booking { Id = _bookingId, User = user, Status = BookingStatus.Pending };

        _mockBookingRepository.Setup(r => r.GetByIdAsync(_bookingId, It.IsAny<string[]>(), null, true))
            .ReturnsAsync(booking);

        var command = new CancelBookingCommand(_bookingId)
        {
            UserDTo = new UserDto
            {
                Id = _userId,
                Roles = new[] {
                    new UserRoleDto()
                    {
                        Role = new () { Name = RoleName.CUSTOMER }
                    }
                }
            }
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.SuccessType.Should().Be(ResultSuccessType.Updated);
        booking.Status.Should().Be(BookingStatus.Cancelled);

        _mockBookingRepository.Verify(r => r.UpdateAsync(booking, false), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookingDoesNotExist_ShouldReturnNotFoundErrorAsync()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.GetByIdAsync(_bookingId, It.IsAny<string[]>(), null, true))
            .ReturnsAsync((Booking)null);

        var command = new CancelBookingCommand(_bookingId)
        {
            UserDTo = new UserDto
            {
                Id = _userId,
                Roles = new[] {
                    new UserRoleDto()
                    {
                        Role = new () { Name = RoleName.CUSTOMER }
                    }
                }
            }
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Errors.First().Description.Should().Be($"Booking with id {_bookingId} not found");
        result.ErrorType.Should().Be(ErrorType.BadRequest);

        _mockBookingRepository.Verify(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotAuthorized_ShouldReturnNotAuthorizedErrorAsync()
    {
        // Arrange
        var differentUserId = Guid.NewGuid();
        var user = new User { Id = differentUserId };
        var booking = new Booking { Id = _bookingId, User = user, Status = BookingStatus.Pending };

        _mockBookingRepository.Setup(r => r.GetByIdAsync(_bookingId, It.IsAny<string[]>(), null, true))
            .ReturnsAsync(booking);

        var command = new CancelBookingCommand(_bookingId)
        {
            UserDTo = new UserDto
            {
                Id = _userId,
                Roles = new[] {
                    new UserRoleDto()
                    {
                        Role = new () { Name = RoleName.CUSTOMER }
                    }
                }
            }
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Errors.First().Description.Should().Be($"User {_userId} is not authorized to cancel booking {_bookingId}");
        result.ErrorType.Should().Be(ErrorType.BadRequest);
        booking.Status.Should().Be(BookingStatus.Pending);

        _mockBookingRepository.Verify(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenBookingIsAlreadyCancelled_ShouldReturnInvalidArgumentErrorAsync()
    {
        // Arrange
        var user = new User { Id = _userId };
        var booking = new Booking { Id = _bookingId, User = user, Status = BookingStatus.Cancelled };

        _mockBookingRepository.Setup(r => r.GetByIdAsync(_bookingId, It.IsAny<string[]>(), null, true))
            .ReturnsAsync(booking);

        var command = new CancelBookingCommand(_bookingId)
        {
            UserDTo = new UserDto
            {
                Id = _userId,
                Roles = new[] {
                    new UserRoleDto()
                    {
                        Role = new () { Name = RoleName.CUSTOMER }
                    }
                }
            }
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Errors.First().Description.Should().Be($"Booking {_bookingId} is already cancelled");
        result.ErrorType.Should().Be(ErrorType.BadRequest);
        booking.Status.Should().Be(BookingStatus.Cancelled);

        _mockBookingRepository.Verify(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<bool>()), Times.Never);
    }
}
