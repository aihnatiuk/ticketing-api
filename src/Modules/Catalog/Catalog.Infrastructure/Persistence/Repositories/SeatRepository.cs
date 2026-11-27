using BuildingBlocks.Infrastructure.Persistence;
using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories;

internal sealed class SeatRepository(CatalogDbContext context)
    : BaseRepository<Seat, long>(context),
        ISeatRepository
{
    public Task<Seat?> GetByIdWithRowAndSectionAsync(
        long id,
        CancellationToken cancellationToken = default
    ) =>
        Entities
            .Include(s => s.Row)
                .ThenInclude(r => r!.Section)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public Task<bool> LabelExistsAsync(
        long rowId,
        string label,
        long? excludeSeatId = null,
        CancellationToken cancellationToken = default
    ) =>
        Entities.AnyAsync(
            s => s.RowId == rowId && s.Label == label && s.Id != excludeSeatId,
            cancellationToken
        );

    public async Task<IReadOnlyList<string>> GetExistingLabelsAsync(
        long rowId,
        IReadOnlyCollection<string> labels,
        CancellationToken cancellationToken = default
    ) =>
        await Entities
            .Where(s => s.RowId == rowId && labels.Contains(s.Label))
            .Select(s => s.Label)
            .ToArrayAsync(cancellationToken);
}
