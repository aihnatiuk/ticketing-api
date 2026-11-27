using BuildingBlocks.Application.Pagination;
using Catalog.Application.Features.Seats.Common;

namespace Catalog.Application.Features.Seats.GetSeats;

public sealed record GetSeatsQuery(long RowId, int Page = 1, int PageSize = 20)
    : PagedQuery<SeatDto>(Page, PageSize);
