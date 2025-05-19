using System;
using BookMe.Application.Commands;
using BookMe.Application.Enums;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookMe.IntegrationTests;

public class UserTests : BaseIntegrationTest
{
    private IMediator _mediator;

    public UserTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
        _mediator = _scope.ServiceProvider.GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task GetOrCreateUserCommandHandler_ShouldCreateACustomerUser_WhenUserDoesNotExistAsync()
    {
        // Arrange
        var email = "test.user@example.com";
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
    }

    // [Fact]
    // public async Task GetOrCreateUserCommandHandler_ShouldReturnUser_WhenUserExistsAsync()
    // {

    // }
}
