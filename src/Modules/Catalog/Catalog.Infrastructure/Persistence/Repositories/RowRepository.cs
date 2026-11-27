using BuildingBlocks.Infrastructure.Persistence;
using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories;

internal sealed class RowRepository(CatalogDbContext context)
    : BaseRepository<Row, long>(context),
        IRowRepository
{
    public Task<Row?> GetByIdWithSectionAsync(
        long id,
        CancellationToken cancellationToken = default
    ) => Entities.Include(r => r.Section).FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public Task<bool> LabelExistsAsync(
        long sectionId,
        string label,
        long? excludeRowId = null,
        CancellationToken cancellationToken = default
    ) =>
        Entities.AnyAsync(
            r => r.SectionId == sectionId && r.Label == label && r.Id != excludeRowId,
            cancellationToken
        );

    public async Task<IReadOnlyList<string>> GetExistingLabelsAsync(
        long sectionId,
        IReadOnlyCollection<string> labels,
        CancellationToken cancellationToken = default
    ) =>
        await Entities
            .Where(r => r.SectionId == sectionId && labels.Contains(r.Label))
            .Select(r => r.Label)
            .ToArrayAsync(cancellationToken);

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
