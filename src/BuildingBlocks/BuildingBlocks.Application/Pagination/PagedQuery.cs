using BuildingBlocks.Application.Abstractions;

namespace BuildingBlocks.Application.Pagination;

/// <summary>
/// Base for queries returning a page of items.
/// </summary>
/// <param name="Page">One-based page number.</param>
/// <param name="PageSize">Requested number of items per page.</param>
/// <param name="MaxPageSize">Upper limit of the page size allowed by the query.</param>
public abstract record PagedQuery<TItem>(
    int Page,
    int PageSize,
    int MaxPageSize = PagedQuery<TItem>.DefaultMaxPageSize
) : IQuery<PagedResult<TItem>>
{
    public const int DefaultMaxPageSize = 100;

    public int Page { get; } = Math.Max(Page, 1);

    public int PageSize { get; } = Math.Clamp(PageSize, 1, Math.Max(MaxPageSize, 1));

    public int Skip => (Page - 1) * PageSize;
}
