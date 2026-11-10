using BuildingBlocks.Infrastructure.Persistence;
using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories;

internal sealed class RowRepository(CatalogDbContext context)
    : BaseRepository<Row, long>(context),
        IRowRepository
{
    public async Task RemoveBySectionAsync(
        long sectionId,
        CancellationToken cancellationToken = default
    )
    {
        Row[] rows = await Entities
            .Where(r => r.SectionId == sectionId)
            .ToArrayAsync(cancellationToken);

        Entities.RemoveRange(rows);
    }
}
