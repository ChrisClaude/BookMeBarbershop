using System;
using BookMe.Application.Interfaces;
using MediatR;
using Serilog;

namespace BookMe.Application.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ITransactionManager _transactionManager;

    public TransactionBehavior(ITransactionManager transactionManager)
    {
        _transactionManager = transactionManager;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var typeName = typeof(TRequest).Name;
        Guid? transactionId = null;

        try
        {
            if (!_transactionManager.HasActiveTransaction)
            {
                transactionId = await _transactionManager.BeginTransactionAsync
                (cancellationToken);
                Log.Information("Transaction started with ID: {TransactionId} for {CommandType}", transactionId, typeName);
            }

            var response = await next(cancellationToken);

            Log.Information("Committing transaction for {CommandType} with ID: {TransactionId}", typeName, transactionId);
            await _transactionManager.CommitTransactionAsync(cancellationToken);

            return response;
        }
        catch (Exception ex)
        {
            await _transactionManager.RollbackTransactionAsync(cancellationToken);
            Log.Error(ex, "Transaction rolled back for command {@Command} with ID: {TransactionId}", request, transactionId);
            throw;
        }
    }
}
