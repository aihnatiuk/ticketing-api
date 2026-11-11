using BuildingBlocks.Application.Pagination;
using Catalog.Application.Features.Events.Common;

namespace Catalog.Application.Features.Events.GetEvents;

public sealed record GetEventsQuery(long? VenueId, int Page = 1, int PageSize = 20)
    : PagedQuery<EventDto>(Page, PageSize);
