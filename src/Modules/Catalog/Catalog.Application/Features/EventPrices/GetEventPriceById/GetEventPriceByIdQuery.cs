using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Features.EventPrices.Common;

namespace Catalog.Application.Features.EventPrices.GetEventPriceById;

public sealed record GetEventPriceByIdQuery(long Id) : IQuery<EventPriceDto>;
