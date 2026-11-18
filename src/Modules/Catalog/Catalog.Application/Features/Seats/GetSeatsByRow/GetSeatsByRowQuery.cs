using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Features.Seats.Common;

namespace Catalog.Application.Features.Seats.GetSeats;

public sealed record GetSeatsByRowQuery(long RowId) : IQuery<IReadOnlyList<SeatDto>>;
