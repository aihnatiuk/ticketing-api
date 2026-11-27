namespace Catalog.Application.Features.Venues.Common.Layout;

internal interface IVenueLayoutValidator
{
    /// <summary>
    /// Checks that the venue layout is complete enough to host events.
    /// </summary>
    /// <returns>The violations found; empty when the layout is valid.</returns>
    Task<IReadOnlyList<LayoutViolation>> ValidateAsync(
        long venueId,
        int maxLabelsPerViolation = 10,
        CancellationToken cancellationToken = default
    );
}
