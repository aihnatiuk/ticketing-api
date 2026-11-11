using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Pagination;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.Events.Common;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Events.GetEvents;

internal sealed class GetEventsQueryHandler(ICatalogDbContext db)
    : IQueryHandler<GetEventsQuery, PagedResult<EventDto>>
{
    public async Task<Result<PagedResult<EventDto>>> Handle(
        GetEventsQuery query,
        CancellationToken cancellationToken = default
    )
    {
        IQueryable<Event> events = db
            .Events.AsNoTracking()
            .Where(e => query.VenueId == null || e.VenueId == query.VenueId);
        int totalCount = await events.CountAsync(cancellationToken);

        List<Event> pageOfEvents = await events
            .OrderBy(e => e.StartsAt)
            .ThenBy(e => e.Id)
            .Skip(query.Skip)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        List<EventDto> items = [.. pageOfEvents.Select(EventDto.FromDomain)];

        return new PagedResult<EventDto>(items, query.Page, query.PageSize, totalCount);
    }
}
