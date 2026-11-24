using BuildingBlocks.Application.Abstractions;
using Microsoft.EntityFrameworkCore.Storage;

namespace BuildingBlocks.Infrastructure.Persistence;

internal sealed class EFCoreTransaction(IDbContextTransaction inner) : ITransaction
{
    public Task CommitAsync(CancellationToken cancellationToken = default) =>
        inner.CommitAsync(cancellationToken);

    public Task RollbackAsync(CancellationToken cancellationToken = default) =>
        inner.RollbackAsync(cancellationToken);

    public ValueTask DisposeAsync() => inner.DisposeAsync();
}
