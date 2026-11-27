using BuildingBlocks.Application.Pagination;
using Catalog.Application.Features.Venues.Common;

namespace Catalog.Application.Features.Venues.GetVenues;

public sealed record GetVenuesQuery(int Page = 1, int PageSize = 20)
    : PagedQuery<VenueDto>(Page, PageSize);
