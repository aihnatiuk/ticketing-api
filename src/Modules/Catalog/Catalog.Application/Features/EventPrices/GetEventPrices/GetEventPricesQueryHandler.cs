using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.EventPrices.Common;
using Catalog.Application.Features.Events;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.EventPrices.GetEventPrices;

internal sealed class GetEventPricesQueryHandler(ICatalogDbContext db)
    : IQueryHandler<GetEventPricesQuery, IReadOnlyList<EventPriceDto>>
{
    public async Task<Result<IReadOnlyList<EventPriceDto>>> Handle(
        GetEventPricesQuery query,
        CancellationToken cancellationToken = default
    )
    {
        if (!await db.Events.AnyAsync(e => e.Id == query.EventId, cancellationToken))
        {
            return EventErrors.NotFound(query.EventId);
        }

        List<EventPrice> eventPrices = await db
            .EventPrices.AsNoTracking()
            .Where(p => p.EventId == query.EventId)
            .OrderBy(p => p.Id)
            .ToListAsync(cancellationToken);

        IReadOnlyList<EventPriceDto> items = [.. eventPrices.Select(EventPriceDto.FromDomain)];

        return Result<IReadOnlyList<EventPriceDto>>.Success(items);
    }
}
