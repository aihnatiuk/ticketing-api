using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Features.Seats.Common;

namespace Catalog.Application.Features.Seats.GetSeatById;

public sealed record GetSeatByIdQuery(long Id) : IQuery<SeatDto>;
