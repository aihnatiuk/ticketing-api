using BuildingBlocks.Domain;

namespace Catalog.Domain.Entities;

public sealed class Seat : Entity<long>
{
    public required string Label { get; set; }

    public required long RowId { get; set; }

    public Row? Row { get; set; }
}
