using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Rows.Common;

public sealed record RowDto(long Id, long SectionId, string Label)
{
    internal static RowDto FromDomain(Row row) => new(row.Id, row.SectionId, row.Label);
}
