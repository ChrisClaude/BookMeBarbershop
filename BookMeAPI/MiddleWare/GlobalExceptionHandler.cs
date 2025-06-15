using System.Net;
using BookMe.Application.Common;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BookMeAPI.MiddleWare;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        if (exception is ValidationException validationException)
        {
            Log.Warning(
                "Validation exception occurred {@ValidationException}",
                validationException
            );

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            var errors = validationException
                .Errors.Select(x => new Error(x.Severity.ToString(), x.ErrorMessage))
                .ToList();

            await httpContext.Response.WriteAsJsonAsync(errors, cancellationToken);

            return true;
        }

        Log.Error(exception, "An unhandled exception occurred");

        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "An unexpected error occurred",
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
