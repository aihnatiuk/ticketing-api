using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.Seats.Common;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Seats.GetSeatById;

internal sealed class GetSeatByIdQueryHandler(ICatalogDbContext db)
    : IQueryHandler<GetSeatByIdQuery, SeatDto>
{
    public async Task<Result<SeatDto>> HandleAsync(
        GetSeatByIdQuery query,
        CancellationToken cancellationToken = default
    )
    {
        Seat? seat = await db
            .Seats.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == query.Id, cancellationToken);

        if (seat is null)
        {
            return SeatErrors.NotFound(query.Id);
        }

        return SeatDto.FromDomain(seat);
    }
}
