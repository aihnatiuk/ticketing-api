namespace BuildingBlocks.Domain;

public abstract class Entity<TId>
    where TId : struct
{
    public virtual TId Id { get; init; }
}
