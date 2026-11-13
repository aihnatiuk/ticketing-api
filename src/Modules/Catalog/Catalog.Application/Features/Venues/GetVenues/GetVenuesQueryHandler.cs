using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Pagination;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.Venues.Common;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Venues.GetVenues;

internal sealed class GetVenuesQueryHandler(ICatalogDbContext db)
    : IQueryHandler<GetVenuesQuery, PagedResult<VenueDto>>
{
    public async Task<Result<PagedResult<VenueDto>>> HandleAsync(
        GetVenuesQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var venues = db.Venues.AsNoTracking();
        int totalCount = await venues.CountAsync(cancellationToken);

        List<Venue> pageOfVenues = await venues
            .OrderBy(v => v.Name)
            .ThenBy(v => v.Id)
            .Skip(query.Skip)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        List<VenueDto> items = [.. pageOfVenues.Select(VenueDto.FromDomain)];

        return new PagedResult<VenueDto>(items, query.Page, query.PageSize, totalCount);
    }
}
