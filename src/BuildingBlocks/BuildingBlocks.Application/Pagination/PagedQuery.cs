using BuildingBlocks.Application.Abstractions;

namespace BuildingBlocks.Application.Pagination;

/// <summary>
/// Base for queries returning a page of items.
/// </summary>
public abstract record PagedQuery<TItem> : IQuery<PagedResult<TItem>>
{
    /// <summary>
    /// Base constructor for queries returning a page of items.
    /// </summary>
    /// <param name="page">One-based page number.</param>
    /// <param name="pageSize">Requested number of items per page.</param>
    /// <param name="maxPageSize">Upper limit of the page size allowed by the query.</param>
    protected PagedQuery(int page, int pageSize, int maxPageSize = 100)
    {
        PageSize = Math.Clamp(pageSize, 1, Math.Max(maxPageSize, 1));
        Page = Math.Clamp(page, 1, int.MaxValue / PageSize); // prevent integer overflow for the "Skip" property.
    }

    public int Page { get; }

    public int PageSize { get; }

    public int Skip => (Page - 1) * PageSize;
}
