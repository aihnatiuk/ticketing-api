using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Pagination;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.Rows;
using Catalog.Application.Features.Seats.Common;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Seats.GetSeats;

internal sealed class GetSeatsQueryHandler(ICatalogDbContext db)
    : IQueryHandler<GetSeatsQuery, PagedResult<SeatDto>>
{
    public async Task<Result<PagedResult<SeatDto>>> Handle(
        GetSeatsQuery query,
        CancellationToken cancellationToken = default
    )
    {
        if (!await db.Rows.AnyAsync(r => r.Id == query.RowId, cancellationToken))
        {
            return RowErrors.NotFound(query.RowId);
        }

        var seats = db.Seats.AsNoTracking().Where(s => s.RowId == query.RowId);
        int totalCount = await seats.CountAsync(cancellationToken);

        List<Seat> pageOfSeats = await seats
            .OrderBy(s => s.Label)
            .ThenBy(s => s.Id)
            .Skip(query.Skip)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        List<SeatDto> items = [.. pageOfSeats.Select(SeatDto.FromDomain)];

        return new PagedResult<SeatDto>(items, query.Page, query.PageSize, totalCount);
    }
}
