using System;
using FluentValidation;
using MediatR;
using Serilog;

namespace BookMe.Application.Behaviors;

public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidatorBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var typeName = typeof(TRequest).Name;

        var failures = _validators
            .Select(v => v.Validate(request))
            .SelectMany(result => result.Errors)
            .Where(error => error != null)
            .ToList();

        if (failures.Any())
        {
            Log.Warning(
                "Validation errors - {CommandType} - Command: {@Command} - Errors: {@ValidationErrors}",
                typeName,
                request,
                failures
            );
            throw new ValidationException(failures);
        }

        return await next(cancellationToken);
    }
}
