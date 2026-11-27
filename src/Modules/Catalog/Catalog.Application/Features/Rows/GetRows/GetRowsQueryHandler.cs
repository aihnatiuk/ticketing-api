using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Pagination;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.Rows.Common;
using Catalog.Application.Features.Sections;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Rows.GetRows;

internal sealed class GetRowsQueryHandler(ICatalogDbContext db)
    : IQueryHandler<GetRowsQuery, PagedResult<RowDto>>
{
    public async Task<Result<PagedResult<RowDto>>> HandleAsync(
        GetRowsQuery query,
        CancellationToken cancellationToken = default
    )
    {
        if (!await db.Sections.AnyAsync(s => s.Id == query.SectionId, cancellationToken))
        {
            return SectionErrors.NotFound(query.SectionId);
        }

        var rows = db.Rows.AsNoTracking().Where(r => r.SectionId == query.SectionId);
        int totalCount = await rows.CountAsync(cancellationToken);

        List<Row> pageOfRows = await rows.OrderBy(r => r.Label)
            .ThenBy(r => r.Id)
            .Skip(query.Skip)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        List<RowDto> items = [.. pageOfRows.Select(RowDto.FromDomain)];

        return new PagedResult<RowDto>(items, query.Page, query.PageSize, totalCount);
    }
}
