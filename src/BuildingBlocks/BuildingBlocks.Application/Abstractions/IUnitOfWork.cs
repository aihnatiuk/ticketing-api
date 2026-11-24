using BuildingBlocks.Contracts;

namespace BuildingBlocks.Application.Abstractions;

/// <summary>
/// Commits the changes of a single module.
/// </summary>
/// <typeparam name="TModule">A module marker type, allowing each module to register its own UoW in a common DI container. </typeparam>
public interface IUnitOfWork<TModule>
    where TModule : IModule
{
    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
