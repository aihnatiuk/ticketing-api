using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BuildingBlocks.Infrastructure.Persistence;

public sealed class UnitOfWork<TModule>(DbContext context) : IUnitOfWork<TModule>
    where TModule : IModule
{
    public async Task<ITransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default
    )
    {
        IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(
            cancellationToken
        );

        return new EFCoreTransaction(transaction);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
