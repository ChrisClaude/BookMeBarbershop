using System;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;

namespace BookMe.IntegrationTests;

public static class AssertionHelper
{
    public static void ValidateOkResult<TPayload>(
        this IActionResult actionResult,
        Action<TPayload> payloadAssertion
    )
        where TPayload : class
    {
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult as OkObjectResult;

        okResult.Value.Should().NotBeNull();
        okResult.Value.Should().BeOfType<TPayload>();

        var response = okResult.Value as TPayload;

        payloadAssertion(response);
    }

    public static void ValidateCreatedResult(this IActionResult actionResult)
    {
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<CreatedResult>();

        var createdResult = actionResult as CreatedResult;

        createdResult.Value.Should().NotBeNull();
    }

    public static void ValidateNoContentResult(this IActionResult actionResult)
    {
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<NoContentResult>();
    }

    public static void ValidateBadRequestResult<TPayload>(
        this IActionResult actionResult,
        Action<TPayload> payloadAssertion
    )
        where TPayload : class
    {
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<BadRequestObjectResult>();

        var badRequestResult = actionResult as BadRequestObjectResult;

        badRequestResult.Value.Should().NotBeNull();
        badRequestResult.Value.Should().BeOfType<TPayload>();

        var response = badRequestResult.Value as TPayload;

        payloadAssertion(response);
    }
}
