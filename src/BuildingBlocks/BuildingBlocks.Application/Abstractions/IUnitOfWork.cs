namespace BuildingBlocks.Application.Abstractions;

/// <summary>
/// Commits the changes of a single module.
/// </summary>
/// <typeparam name="TModule">A module marker type, allowing each module to register its own UoW in a common DI container. </typeparam>
public interface IUnitOfWork<TModule>
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
