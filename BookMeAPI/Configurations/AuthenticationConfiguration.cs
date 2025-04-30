using BookMe.Application.Commands;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Enums;
using BookMe.Application.Exceptions;
using BookMe.Application.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Serilog;

namespace BookMeAPI.Configurations;

/// <summary>
/// Provides configuration for Azure AD B2C authentication in the application.
/// </summary>
public static class AuthenticationConfiguration
{
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(options =>
            {
                configuration.Bind("AppSettings:AzureAdB2C", options);
                options.TokenValidationParameters.NameClaimType = "name";
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = HandleOnTokenValidatedAsync,

                };
            }, options => { configuration.Bind("AppSettings:AzureAdB2C", options); });

        return services;
    }

    private static async Task HandleOnTokenValidatedAsync(TokenValidatedContext context)
    {
        try
        {
            var mediator = context.HttpContext.RequestServices.GetRequiredService<IMediator>() ?? throw new Exception("Could not get service to retrieve user.");

            var userEmail = context?.Principal?.Claims.FirstOrDefault(claim => claim.Type == "emails")?.Value;

            var (key, user) = await GetAuthenticateUserWithKeyAsync(mediator, userEmail);

            context?.HttpContext.Items.Add(new KeyValuePair<object, object>(key, user));
        }
        catch (HttpContextUserLoadingProcessFailureException ex)
        {
            Log.Error(ex, "Error occurred during user loading.");

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            if (!context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                await context.Response.StartAsync();
            }

            await context.Response.WriteAsync("Failed to load user: " + ex.Message);

            await context.Response.CompleteAsync();

            context.Fail(ex);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error occurred.");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            if (!context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                await context.Response.StartAsync();
            }

            await context.Response.WriteAsync("An unexpected error occurred: " + ex.Message);

            await context.Response.CompleteAsync();

            context.Fail(ex);
        }
    }

    private static async Task<(string key, UserDto user)> GetAuthenticateUserWithKeyAsync(IMediator mediator, string userEmail)
    {
        // The email is validated as part of the command validation
        var result = await mediator.Send(new GetOrCreateUserCommand(userEmail));

        if (result.IsFailure)
        {
            throw new HttpContextUserLoadingProcessFailureException(result.Errors.ToAggregateString());
        }

        return (Constant.HTTP_CONTEXT_USER_ITEM_KEY, result.Value);
    }
}
