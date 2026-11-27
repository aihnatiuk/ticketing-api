using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Seats.Common;

public sealed record SeatDto(long Id, long RowId, string Label)
{
    internal static SeatDto FromDomain(Seat seat) => new(seat.Id, seat.RowId, seat.Label);
}
