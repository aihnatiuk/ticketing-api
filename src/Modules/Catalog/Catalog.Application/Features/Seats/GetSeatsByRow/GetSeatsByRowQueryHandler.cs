using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.Rows;
using Catalog.Application.Features.Seats.Common;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Seats.GetSeats;

internal sealed class GetSeatsByRowQueryHandler(ICatalogDbContext db)
    : IQueryHandler<GetSeatsByRowQuery, IReadOnlyList<SeatDto>>
{
    public async Task<Result<IReadOnlyList<SeatDto>>> HandleAsync(
        GetSeatsByRowQuery query,
        CancellationToken cancellationToken = default
    )
    {
        if (!await db.Rows.AnyAsync(r => r.Id == query.RowId, cancellationToken))
        {
            return RowErrors.NotFound(query.RowId);
        }

        List<Seat> seats = await db
            .Seats.AsNoTracking()
            .Where(s => s.RowId == query.RowId)
            .OrderBy(s => s.Label)
            .ThenBy(s => s.Id)
            .ToListAsync(cancellationToken);

        return seats.Select(SeatDto.FromDomain).ToList();
    }
}
