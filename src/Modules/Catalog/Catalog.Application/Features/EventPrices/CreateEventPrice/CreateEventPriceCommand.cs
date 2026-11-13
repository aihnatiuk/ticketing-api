using BuildingBlocks.Application.Abstractions;

namespace Catalog.Application.Features.EventPrices.CreateEventPrice;

public sealed record CreateEventPriceCommand(
    long EventId,
    long PricingZoneId,
    long TicketTypeId,
    decimal Amount
) : ICommand<long>;
