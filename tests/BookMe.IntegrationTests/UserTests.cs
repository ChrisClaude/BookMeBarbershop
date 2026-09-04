using System.Net.Http.Json;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Common.Dtos.Users;
using BookMe.Application.Entities;
using BookMe.Application.Enums;
using BookMe.Application.Mappings;
using BookMe.IntegrationTests.TestData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BookMe.IntegrationTests;

public class UserTests : BaseIntegrationTest
{
    private readonly UserDto _adminUser;

    public UserTests(AspireIntegrationTestFixture factory)
        : base(factory)
    {
        _adminUser = _bookMeContext
            .Users.Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .First(x => x.UserRoles.Any(y => y.RoleId == DefaultRoles.AdminId))
            .MapToDto();
    }

    [Fact]
    public async Task GetOrCreateUserCommandHandler_ShouldCreateACustomerUser_WhenUserDoesNotExistAsync()
    {
        // Arrange
        var email = "test.user@eagle.com";
        await _bookMeContext.Users.Where(x => x.Email == email).ExecuteDeleteAsync();

        // Auth for a not-yet-existing email runs the same GetOrCreateUser flow as a real
        // first-time login (see TestAuthHandler / AuthenticatedUserLoader).
        SetUser(new UserDto { Email = email });

        // Act
        var response = await _client.GetAsync("api/user/me");

        // Assert
        var user = await response.ShouldBeOkAsync<UserDto>();
        user.Email.Should().Be(email);
        user.Name.Should().Be("test.user");
        user.Surname.Should().Be("test.user");
        user.Roles.Should().HaveCount(1);
        user.Roles.ToList()[0].Role.Name.Should().Be(RoleName.CUSTOMER);

        _bookMeContext.Users.Where(x => x.Email == email).Should().HaveCount(1);

        await TestDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
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

        SetUser(new UserDto { Email = email });

        // Act
        var response = await _client.GetAsync("api/user/me");

        // Assert
        var userDto = await response.ShouldBeOkAsync<UserDto>();
        userDto.Email.Should().Be(email);
        userDto.Name.Should().Be("test.user");
        userDto.Surname.Should().Be("test.user");
        userDto.Roles.Should().HaveCount(1);
        userDto.Roles.ToList()[0].Role.Name.Should().Be(RoleName.CUSTOMER);

        _bookMeContext.Users.Where(x => x.Email == email).Should().HaveCount(1);

        await TestDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task UpdateUserCommandHandler_ShouldUpdateUser_WhenUserExistsAsync()
    {
        // Arrange
        var email = "test.update.user@eagle.com";
        await _bookMeContext.Users.Where(x => x.Email == email).ExecuteDeleteAsync();

        // The first authenticated request for this email creates the user (GetOrCreateUser).
        SetUser(new UserDto { Email = email });
        var updateUserRequest = new UserUpdateDto { Name = "Chris", Surname = "Claude" };

        // Act
        var response = await _client.PutAsJsonAsync("api/user/profile", updateUserRequest);

        // Assert
        var user = await response.ShouldBeOkAsync<UserDto>();
        user.Name.Should().Be("Chris");
        user.Surname.Should().Be("Claude");
        user.Email.Should().Be(email);

        var updatedUser = await _bookMeContext.Users.FirstAsync(x => x.Email == email);
        updatedUser.Name.Should().Be("Chris");
        updatedUser.Surname.Should().Be("Claude");

        await TestDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }

    [Fact]
    public async Task GetUsers_ShouldSucceedAsync()
    {
        // Arrange
        SetUser(_adminUser);

        // Act
        var response = await _client.GetAsync("api/user/all");

        // Assert
        var users = await response.ShouldBeOkAsync<PagedListDto<UserDto>>();
        users.Items.Should().HaveCount(2);
        var john = users.Items.First(x => x.Name == "John");
        var jane = users.Items.First(x => x.Name == "Jane");
        john.Roles.Count().Should().Be(1);
        john.Roles.First().Role.Name.Should().Be(RoleName.ADMIN);
        jane.Roles.Count().Should().Be(1);
        jane.Roles.First().Role.Name.Should().Be(RoleName.CUSTOMER);

        await TestDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }
}