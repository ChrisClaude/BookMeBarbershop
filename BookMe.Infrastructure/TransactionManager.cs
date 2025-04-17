using System;
using System.Data;
using BookMe.Application.Interfaces;
using BookMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookMe.Infrastructure;

public class TransactionManager(BookMeContext context) : ITransactionManager
{
    private IDbContextTransaction _transaction;

    public bool HasActiveTransaction => _transaction != null;

    public async Task<Guid?> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
            return null;

        _transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        return _transaction.TransactionId;
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
            throw new InvalidOperationException("No active transaction to commit.");
        return _transaction.CommitAsync(cancellationToken);
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
            throw new InvalidOperationException("No active transaction to rollback.");
        return _transaction.RollbackAsync(cancellationToken);
    }
}
