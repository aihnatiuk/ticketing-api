using BuildingBlocks.Domain;

namespace BuildingBlocks.Application.Abstractions;

public interface IRepository<TEntity, in TId>
    where TEntity : Entity<TId>
    where TId : struct
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    void Add(TEntity entity);

    void Remove(TEntity entity);
}
