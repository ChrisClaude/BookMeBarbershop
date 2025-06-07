using BookMe.Application.Commands;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Common.Dtos.Users;
using BookMe.Application.Entities;
using BookMe.Application.Enums;
using BookMe.Application.Interfaces.Queries;
using BookMe.IntegrationTests.TestData;
using BookMeAPI.Apis;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookMe.IntegrationTests;

public class UserTests : BaseIntegrationTest
{
    private IMediator _mediator;
    private UserController _userController;

    public UserTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
        _mediator = _scope.ServiceProvider.GetRequiredService<IMediator>();
        var userQueries = _scope.ServiceProvider.GetRequiredService<IUserQueries>();
        _userController = new UserController(_mediator, userQueries);
    }

    [Fact]
    public async Task GetOrCreateUserCommandHandler_ShouldCreateACustomerUser_WhenUserDoesNotExistAsync()
    {
        // Arrange
        var email = "test.user@eagle.com";
        await _bookMeContext.Users.Where(x => x.Email == email).ExecuteDeleteAsync();

        // Act
        var result = await _mediator.Send(new GetOrCreateUserCommand(email));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be(email);
        result.Value.Name.Should().Be("test.user");
        result.Value.Surname.Should().Be("test.user");
        result.Value.Roles.Should().HaveCount(1);
        result.Value.Roles.ToList()[0].Role.Name.Should().Be(RoleName.CUSTOMER);

        _bookMeContext.Users.Where(x => x.Email == email).Should().HaveCount(1);

        await TestCDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task GetOrCreateUserCommandHandler_ShouldReturnUser_WhenUserExistsAsync()
    {
        // Arrange
        var email = "test.user@eagle.com";
        await _bookMeContext.Users.Where(x => x.Email == email).ExecuteDeleteAsync();

        var user = new User
        {
            Email = email,
            Name = "test.user",
            Surname = "test.user",
            UserRoles = new List<UserRole> { new() { RoleId = DefaultRoles.CustomerId } },
        };

        await _bookMeContext.Users.AddAsync(user);
        await _bookMeContext.SaveChangesAsync();

        // Act
        var result = await _mediator.Send(new GetOrCreateUserCommand(email));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be(email);
        result.Value.Name.Should().Be("test.user");
        result.Value.Surname.Should().Be("test.user");
        result.Value.Roles.Should().HaveCount(1);
        result.Value.Roles.ToList()[0].Role.Name.Should().Be(RoleName.CUSTOMER);

        _bookMeContext.Users.Where(x => x.Email == email).Should().HaveCount(1);

        await TestCDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task UpdateUserCommandHandler_ShouldUpdateUser_WhenUserExistsAsync()
    {
        // Arrange
        var email = "test.update.user@eagle.com";
        await _bookMeContext.Users.Where(x => x.Email == email).ExecuteDeleteAsync();

        var createResult = await _mediator.Send(new GetOrCreateUserCommand(email));
        var userDto = createResult.Value;
        _mockHttpContext.SetUser(userDto);
        var updateUserRequest = new UserUpdateDto { Name = "Chris", Surname = "Claude" };

        // Act
        var result = await _userController.UpdateProfileAsync(updateUserRequest);

        // Assert
        result.ValidateOkResult<UserDto>(user =>
        {
            user.Name.Should().Be("Chris");
            user.Surname.Should().Be("Claude");
            user.Email.Should().Be(email);
        });

        var updatedUser = await _bookMeContext.Users.FirstAsync(x => x.Id == userDto.Id);
        updatedUser.Name.Should().Be("Chris");
        updatedUser.Surname.Should().Be("Claude");

        await TestCDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }
}
