using BuildingBlocks.Application.Pagination;
using Catalog.Application.Features.Rows.Common;

namespace Catalog.Application.Features.Rows.GetRows;

public sealed record GetRowsQuery(long SectionId, int Page = 1, int PageSize = 20)
    : PagedQuery<RowDto>(Page, PageSize);
