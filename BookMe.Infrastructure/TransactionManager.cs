using BookMe.Application.Interfaces;
using BookMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookMe.Infrastructure;

public class TransactionManager : ITransactionManager, IDisposable, IAsyncDisposable
{
    private readonly BookMeContext _context;
    private IDbContextTransaction _transaction;
    private bool _disposed;

    public TransactionManager(BookMeContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public bool HasActiveTransaction => _transaction != null;

    public async Task<Guid?> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (_transaction != null)
            return _transaction.TransactionId;

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        return _transaction.TransactionId;
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (_transaction == null)
            throw new InvalidOperationException("No active transaction to commit.");

        try
        {
            await _transaction.CommitAsync(cancellationToken);
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (_transaction == null)
            throw new InvalidOperationException("No active transaction to rollback.");

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    private async Task DisposeTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(TransactionManager));
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _transaction?.Dispose();
        }

        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
            }

            _disposed = true;
        }

        GC.SuppressFinalize(this);
    }
}
