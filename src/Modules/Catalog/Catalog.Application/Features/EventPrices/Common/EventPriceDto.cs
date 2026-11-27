using Catalog.Domain.Entities;

namespace Catalog.Application.Features.EventPrices.Common;

public sealed record EventPriceDto(
    long Id,
    long EventId,
    long PricingZoneId,
    long TicketTypeId,
    decimal Amount
)
{
    internal static EventPriceDto FromDomain(EventPrice eventPrice) =>
        new(
            eventPrice.Id,
            eventPrice.EventId,
            eventPrice.PricingZoneId,
            eventPrice.TicketTypeId,
            eventPrice.Amount
        );
}
