using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Features.EventPrices.Common;

namespace Catalog.Application.Features.EventPrices.GetEventPrices;

public sealed record GetEventPricesQuery(long EventId) : IQuery<IReadOnlyList<EventPriceDto>>;
