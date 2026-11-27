using Catalog.Application.Abstractions;
using Catalog.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Events.PublishEvent;

internal sealed class EventPublishingReader(ICatalogDbContext db) : IEventPublishingReader
{
    public async Task<IReadOnlyList<long>> GetZonesWithoutPricesAsync(
        long eventId,
        long venueId,
        CancellationToken cancellationToken = default
    )
    {
        return await db
            .Sections.AsNoTracking()
            .Where(s => s.VenueId == venueId)
            .Select(s => s.PricingZoneId)
            .Distinct()
            .Where(zoneId =>
                !db.EventPrices.Any(p => p.EventId == eventId && p.PricingZoneId == zoneId)
            )
            .OrderBy(zoneId => zoneId)
            .ToListAsync(cancellationToken);
    }

    public async Task<EventAvailabilitySnapshot> GetAvailabilitySnapshotAsync(
        long venueId,
        CancellationToken cancellationToken = default
    )
    {
        List<long> seatIds = await db
            .Seats.AsNoTracking()
            .Where(s =>
                s.Row!.Section!.VenueId == venueId
                && s.Row.Section.Type == SectionType.ReservedSeating
            )
            .OrderBy(s => s.Id)
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        List<GeneralAdmissionAvailability> generalAdmissionSections = await db
            .Sections.AsNoTracking()
            .Where(s => s.VenueId == venueId && s.Type == SectionType.GeneralAdmission)
            .OrderBy(s => s.Id)
            .Select(s => new GeneralAdmissionAvailability(s.Id, s.Capacity!.Value))
            .ToListAsync(cancellationToken);

        return new EventAvailabilitySnapshot(seatIds, generalAdmissionSections);
    }
}
