using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.Rows.Common;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Rows.GetRowById;

internal sealed class GetRowByIdQueryHandler(ICatalogDbContext db)
    : IQueryHandler<GetRowByIdQuery, RowDto>
{
    public async Task<Result<RowDto>> HandleAsync(
        GetRowByIdQuery query,
        CancellationToken cancellationToken = default
    )
    {
        Row? row = await db
            .Rows.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == query.Id, cancellationToken);

        if (row is null)
        {
            return RowErrors.NotFound(query.Id);
        }

        return RowDto.FromDomain(row);
    }
}
