using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Pagination;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.Sections.Common;
using Catalog.Application.Features.Venues;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Sections.GetSections;

internal sealed class GetSectionsQueryHandler(ICatalogDbContext db)
    : IQueryHandler<GetSectionsQuery, PagedResult<SectionDto>>
{
    public async Task<Result<PagedResult<SectionDto>>> HandleAsync(
        GetSectionsQuery query,
        CancellationToken cancellationToken = default
    )
    {
        if (!await db.Venues.AnyAsync(v => v.Id == query.VenueId, cancellationToken))
        {
            return VenueErrors.NotFound(query.VenueId);
        }

        var sections = db.Sections.AsNoTracking().Where(s => s.VenueId == query.VenueId);
        int totalCount = await sections.CountAsync(cancellationToken);

        List<Section> pageOfSections = await sections
            .OrderBy(s => s.Label)
            .ThenBy(s => s.Id)
            .Skip(query.Skip)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        List<SectionDto> items = [.. pageOfSections.Select(SectionDto.FromDomain)];

        return new PagedResult<SectionDto>(items, query.Page, query.PageSize, totalCount);
    }
}
