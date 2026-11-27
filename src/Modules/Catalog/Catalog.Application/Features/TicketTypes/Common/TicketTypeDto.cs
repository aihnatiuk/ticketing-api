using Catalog.Domain.Entities;

namespace Catalog.Application.Features.TicketTypes.Common;

public sealed record TicketTypeDto(long Id, string Label)
{
    internal static TicketTypeDto FromDomain(TicketType ticketType) =>
        new(ticketType.Id, ticketType.Label);
}
