using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Persistence;

public class BaseRepository<TEntity, TId>(DbContext context) : IRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : struct
{
    protected DbSet<TEntity> Entities { get; } = context.Set<TEntity>();

    public virtual async Task<TEntity?> GetByIdAsync(
        TId id,
        CancellationToken cancellationToken = default
    ) => await Entities.FindAsync([id], cancellationToken);

    public async Task<TEntity[]> GetByIdsAsync(
        IEnumerable<TId> ids,
        CancellationToken cancellationToken = default
    ) => await Entities.Where(e => ids.Contains(e.Id)).ToArrayAsync(cancellationToken);

    public virtual void Add(TEntity entity) => Entities.Add(entity);

    public virtual void Remove(TEntity entity) => Entities.Remove(entity);
}
