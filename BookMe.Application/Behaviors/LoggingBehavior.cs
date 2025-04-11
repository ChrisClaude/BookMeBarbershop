using System;
using MediatR;
using Serilog;

namespace BookMe.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        Log.Information("Handling {RequestName} ({@Request})", request.GetType().Name, request);

        var response = await next(cancellationToken);

        Log.Information("Handled {RequestName} ({@Response})", request.GetType().Name, response);

        return response;
    }
}
