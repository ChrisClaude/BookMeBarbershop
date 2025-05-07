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
        var createUserCommand = new CreateOrUpdateUserCommand("John", "Doe", "john.doe@example.com", "1000000000");

        // Act
        var result = await _handler.Handle(createUserCommand, CancellationToken.None);

        // Assert
        _repository.Verify(x => x.InsertAsync(It.Is<User>(x => (
            x.Name == "John" &&
            x.Surname == "Doe" &&
            x.Email == "john.doe@example.com" &&
            x.PhoneNumber == "1000000000" &&
            x.UserRoles.Count == 1 &&
            x.UserRoles[0].RoleId == DefaultRoles.CustomerId
        )), true), Times.Once);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType(typeof(UserDto));
        result.Value.Roles.Should().Contain(RoleName.CUSTOMER);
    }
}
