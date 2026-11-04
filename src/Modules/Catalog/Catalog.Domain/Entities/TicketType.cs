using BuildingBlocks.Domain;

namespace Catalog.Domain.Entities;

public sealed class TicketType : Entity<long>
{
    public required string Label { get; set; }
}
