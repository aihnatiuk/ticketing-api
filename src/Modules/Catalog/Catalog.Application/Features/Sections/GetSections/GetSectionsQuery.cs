using BuildingBlocks.Application.Pagination;
using Catalog.Application.Features.Sections.Common;

namespace Catalog.Application.Features.Sections.GetSections;

public sealed record GetSectionsQuery(long VenueId, int Page = 1, int PageSize = 20)
    : PagedQuery<SectionDto>(Page, PageSize);
