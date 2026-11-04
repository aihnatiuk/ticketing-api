using BuildingBlocks.Domain;

namespace Catalog.Domain.Entities;

public sealed class Row : Entity<long>
{
    public required string Label { get; set; }

    public required long SectionId { get; set; }

    public Section? Section { get; set; }

    public ICollection<Seat> Seats { get; set; } = [];
}
