using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BookMe.Application.Common;
using FluentAssertions;

namespace BookMe.IntegrationTests;

public static class AssertionHelper
{
    // BadRequest bodies come from two code paths that don't agree on JSON casing:
    // ResultExtensions.ToActionResult -> MVC/Newtonsoft (PascalCase), GlobalExceptionHandler's
    // FluentValidation path -> WriteAsJsonAsync (camelCase). Case-insensitive covers both.
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public static async Task<T> ShouldBeOkAsync<T>(this HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        payload.Should().NotBeNull();

        return payload!;
    }

    public static Task ShouldBeNoContentAsync(this HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        return Task.CompletedTask;
    }

    public static async Task<List<Error>> ShouldBeBadRequestAsync(this HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<List<Error>>(_jsonOptions);
        errors.Should().NotBeNull();

        return errors!;
    }

    public static Task ShouldBeUnauthorizedAsync(this HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        return Task.CompletedTask;
    }

    // The [Authorize(Policy = ...)] check runs before the controller action / FluentValidation
    // pipeline, so a wrong-role user is rejected here with an empty 403, not a 400 with an
    // error body.
    public static Task ShouldBeForbiddenAsync(this HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        return Task.CompletedTask;
    }
}