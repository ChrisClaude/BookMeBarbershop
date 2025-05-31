﻿namespace BookMe.Application.Common;

public sealed record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public override string ToString()
    {
        return $"{Code} - {Description}";
    }

    public static Error InternalServerError(string exceptionMessage = null)
    {
        if (string.IsNullOrEmpty(exceptionMessage))
        {
            exceptionMessage = "Server Error";
        }
        return new("internal-server-error", exceptionMessage);
    }

    public static Error NotFound(string exceptionMessage = null)
    {
        if (string.IsNullOrEmpty(exceptionMessage))
        {
            exceptionMessage = "Not Found";
        }
        return new("not-found", exceptionMessage);
    }

    public static Error NotAuthorized(string exceptionMessage = null)
    {
        if (string.IsNullOrEmpty(exceptionMessage))
        {
            exceptionMessage = "Not Authorized";
        }
        return new("not-authorized", exceptionMessage);
    }

    public static Error InvalidArgument(string exceptionMessage = null)
    {
        if (string.IsNullOrEmpty(exceptionMessage))
        {
            exceptionMessage = "Invalid Argument";
        }
        return new("invalid-argument", exceptionMessage);
    }

    public static Error Conflict(string errorMessage = null)
    {
        if (string.IsNullOrEmpty(errorMessage))
        {
            errorMessage = "Conflict";
        }
        return new("conflict", errorMessage);
    }

    public static Error ExternalServiceError(string errorMessage = null)
    {
        if (string.IsNullOrEmpty(errorMessage))
        {
            errorMessage = "External Service Error";
        }
        return new("external-service-error", errorMessage);
    }

    public static Error TooManyRequestsError(string errorMessage = null)
    {
        if (string.IsNullOrEmpty(errorMessage))
        {
            errorMessage = "Too Many Requests";
        }
        return new("too-many-requests", errorMessage);
    }
}
