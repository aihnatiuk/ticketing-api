using BuildingBlocks.Domain;

namespace Catalog.Domain.Entities;

public sealed class EventPrice : Entity<long>
{
    public required decimal Amount { get; set; }

    public required long EventId { get; set; }

    public required long PricingZoneId { get; set; }

    public required long TicketTypeId { get; set; }

    public TicketType? TicketType { get; set; }
}
