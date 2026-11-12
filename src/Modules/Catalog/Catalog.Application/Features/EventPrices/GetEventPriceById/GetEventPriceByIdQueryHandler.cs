using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.EventPrices.Common;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.EventPrices.GetEventPriceById;

internal sealed class GetEventPriceByIdQueryHandler(ICatalogDbContext db)
    : IQueryHandler<GetEventPriceByIdQuery, EventPriceDto>
{
    public async Task<Result<EventPriceDto>> HandleAsync(
        GetEventPriceByIdQuery query,
        CancellationToken cancellationToken = default
    )
    {
        EventPrice? eventPrice = await db
            .EventPrices.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == query.Id, cancellationToken);

        if (eventPrice is null)
        {
            return EventPriceErrors.NotFound(query.Id);
        }

        return EventPriceDto.FromDomain(eventPrice);
    }
}
