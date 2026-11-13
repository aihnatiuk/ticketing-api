using BuildingBlocks.Infrastructure.Persistence;
using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories;

internal sealed class SectionRepository(CatalogDbContext context)
    : BaseRepository<Section, long>(context),
        ISectionRepository
{
    public Task<bool> LabelExistsAsync(
        long venueId,
        string label,
        long? excludeSectionId = null,
        CancellationToken cancellationToken = default
    ) =>
        Entities.AnyAsync(
            s => s.VenueId == venueId && s.Label == label && s.Id != excludeSectionId,
            cancellationToken
        );

    public async Task<IReadOnlyList<string>> GetExistingLabelsAsync(
        long venueId,
        IReadOnlyCollection<string> labels,
        CancellationToken cancellationToken = default
    ) =>
        await Entities
            .Where(s => s.VenueId == venueId && labels.Contains(s.Label))
            .Select(s => s.Label)
            .ToArrayAsync(cancellationToken);
}
