using Catalog.Application.Abstractions;
using Catalog.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Venues.Common.Layout;

internal sealed class VenueLayoutValidator(ICatalogDbContext db) : IVenueLayoutValidator
{
    public async Task<IReadOnlyList<LayoutViolation>> ValidateAsync(
        long venueId,
        int maxLabelsPerViolation = 10,
        CancellationToken cancellationToken = default
    )
    {
        if (!await db.Sections.AnyAsync(s => s.VenueId == venueId, cancellationToken))
        {
            return [new LayoutViolation(LayoutViolationCode.NoSections, [])];
        }

        List<LayoutViolation> violations = [];

        List<string> seatedWithoutRows = await db
            .Sections.AsNoTracking()
            .Where(s =>
                s.VenueId == venueId && s.Type == SectionType.ReservedSeating && !s.Rows.Any()
            )
            .OrderBy(s => s.Id)
            .Select(s => s.Label)
            .Take(maxLabelsPerViolation)
            .ToListAsync(cancellationToken);

        if (seatedWithoutRows.Count > 0)
        {
            violations.Add(
                new LayoutViolation(LayoutViolationCode.SeatedSectionWithoutRows, seatedWithoutRows)
            );
        }

        List<string> rowsWithoutSeats = await db
            .Rows.AsNoTracking()
            .Where(r =>
                r.Section!.VenueId == venueId
                && r.Section.Type == SectionType.ReservedSeating
                && !r.Seats.Any()
            )
            .OrderBy(r => r.SectionId)
            .ThenBy(r => r.Id)
            .Select(r => r.Section!.Label + " / " + r.Label)
            .Take(maxLabelsPerViolation)
            .ToListAsync(cancellationToken);

        if (rowsWithoutSeats.Count > 0)
        {
            violations.Add(
                new LayoutViolation(LayoutViolationCode.RowWithoutSeats, rowsWithoutSeats)
            );
        }

        return violations;
    }
}
