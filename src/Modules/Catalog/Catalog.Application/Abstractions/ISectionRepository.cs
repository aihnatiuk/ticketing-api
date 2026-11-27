using BuildingBlocks.Application.Abstractions;
using Catalog.Domain.Entities;

namespace Catalog.Application.Abstractions;

public interface ISectionRepository : IRepository<Section, long>
{
    /// <summary>
    /// Checks whether a section with the given label already exists in the venue.
    /// <paramref name="venueId"/>The Id of the venue to check.</paramref>
    /// <paramref name="label"/>The label to check.</paramref>
    /// <paramref name="excludeSectionId"/>An Id of a zone to exclude from the check. Can be used when updating a section's Label to the same value with different casing. If null, all zones will be checked.</paramref>
    /// </summary>
    Task<bool> LabelExistsAsync(
        long venueId,
        string label,
        long? excludeSectionId = null,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<string>> GetExistingLabelsAsync(
        long venueId,
        IReadOnlyCollection<string> labels,
        CancellationToken cancellationToken = default
    );
}
