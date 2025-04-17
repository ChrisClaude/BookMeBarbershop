using System;

namespace BookMe.Application.Interfaces;

public interface ITransactionManager
{
    bool HasActiveTransaction { get; }

    Task<Guid?> BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
