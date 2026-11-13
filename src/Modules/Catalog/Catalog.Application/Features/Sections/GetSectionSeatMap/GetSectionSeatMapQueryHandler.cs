using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.Events;
using Catalog.Application.Features.TicketTypes.Common;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Sections.GetSectionSeatMap;

internal sealed class GetSectionSeatMapQueryHandler(ICatalogDbContext dbContext)
    : IQueryHandler<GetSectionSeatMapQuery, SectionSeatMapDto>
{
    public async Task<Result<SectionSeatMapDto>> HandleAsync(
        GetSectionSeatMapQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var @event = await dbContext
            .Events.Where(e => e.Id == query.EventId)
            .Select(e => new { e.Id, e.VenueId })
            .FirstOrDefaultAsync(cancellationToken);

        if (@event is null)
        {
            return EventErrors.NotFound(query.EventId);
        }

        Section? section = await dbContext
            .Sections.AsNoTracking()
            .Where(s => s.Id == query.SectionId && s.VenueId == @event.VenueId)
            .Include(s => s.PricingZone)
            .FirstOrDefaultAsync(cancellationToken);

        if (section is null)
        {
            return SectionErrors.NotFoundInVenue(query.SectionId, @event.VenueId);
        }

        List<SeatMapPriceDto> sectionPricing = await dbContext
            .EventPrices.Where(x =>
                x.EventId == @event.Id && x.PricingZoneId == section.PricingZoneId
            )
            .Select(x => new SeatMapPriceDto(
                new TicketTypeDto(x.TicketType!.Id, x.TicketType.Label),
                x.Amount
            ))
            .ToListAsync(cancellationToken);

        List<SeatMapRowDto>? rows = null;
        if (section.Type is not SectionType.GeneralAdmission)
        {
            rows = await dbContext
                .Rows.Where(r => r.SectionId == section.Id)
                .OrderBy(r => r.Label)
                .Select(r => new SeatMapRowDto(
                    r.Id,
                    r.Label,
                    r.Seats.OrderBy(s => s.Label)
                        .Select(s => new SeatMapSeatDto(s.Id, s.Label))
                        .ToList()
                ))
                .ToListAsync(cancellationToken);
        }

        return new SectionSeatMapDto(
            @event.Id,
            section.Id,
            section.Type,
            section.PricingZone!.Label,
            sectionPricing,
            section.Capacity,
            rows
        );
    }
}
