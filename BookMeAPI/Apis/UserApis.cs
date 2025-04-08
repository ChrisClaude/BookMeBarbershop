using Microsoft.AspNetCore.Http.HttpResults;

namespace BookMeAPI.Apis;

internal static class UserApis
{
    public static RouteGroupBuilder MapUserApis(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/users");

        app.MapGet("/me", () => GetUserAsync());

        return api;
    }

    public static async Task<Results<Ok<string>, NotFound, ProblemHttpResult>> GetUserAsync()
    {
        return TypedResults.Ok("test user");
    }
}
