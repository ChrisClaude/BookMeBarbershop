using System;
using BookMe.Application.Commands.Users;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Entities;
using BookMe.Application.Enums;
using BookMe.Application.Handlers;
using BookMe.Application.Interfaces;
using Moq;
using FluentAssertions;

namespace BookMe.UnitTests.Application.Handlers;

public class CreateUserCommandHandlerTest
{
    private readonly Mock<IRepository<User>> _repository;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTest()
    {
        _repository = new Mock<IRepository<User>>();
        _handler = new CreateUserCommandHandler(_repository.Object);
    }

    [Fact]
    public async Task CreateUserCommandHandler_ShouldCreateUserWithCustomerRoleAsync()
    {
        // Arrange
        var createUserCommand = new CreateUserCommand(
            "John",
            "Doe",
            "john.doe@example.com",
            "1000000000"
        );

        var userId = Guid.NewGuid();
        var insertedUser = new User
        {
            Id = userId,
            Name = "John",
            Surname = "Doe",
            Email = "john.doe@example.com",
            PhoneNumber = "1000000000",
            UserRoles = new List<UserRole>
            {
                new()
                {
                    RoleId = DefaultRoles.CustomerId,
                    Role = new Role { Id = DefaultRoles.CustomerId, Name = RoleName.CUSTOMER }
                }
            }
        };

        _repository
            .Setup(r => r.InsertAsync(It.IsAny<User>(), false))
            .Callback<User, bool>(
                (user, _) =>
                {
                    user.Id = userId;
                    user.UserRoles[0].Role = new Role
                    {
                        Id = DefaultRoles.CustomerId,
                        Name = RoleName.CUSTOMER
                    };
                }
            );

        _repository
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<string[]>(), null, true))
            .ReturnsAsync(insertedUser);

        // Act
        var result = await _handler.Handle(createUserCommand, CancellationToken.None);

        // Assert
        _repository.Verify(
            x =>
                x.InsertAsync(
                    It.Is<User>(
                        x =>
                            (
                                x.Name == "John"
                                && x.Surname == "Doe"
                                && x.Email == "john.doe@example.com"
                                && x.PhoneNumber == "1000000000"
                                && x.UserRoles.Count == 1
                                && x.UserRoles[0].RoleId == DefaultRoles.CustomerId
                            )
                    ),
                    false
                ),
            Times.Once
        );
        _repository.Verify(
            x =>
                x.GetByIdAsync(
                    userId,
                    It.Is<string[]>(
                        x =>
                            (
                                x.Length == 2
                                && x[0] == nameof(User.UserRoles)
                                && x[1] == $"{nameof(User.UserRoles)}.{nameof(UserRole.Role)}"
                            )
                    ),
                    null,
                    true
                ),
            Times.Once
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType(typeof(UserDto));
        result.Value.Roles.Count().Should().Be(1);
        result.Value.Roles.First().Role.Name.Should().Be(RoleName.CUSTOMER);
    }
}
