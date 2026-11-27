using BuildingBlocks.Domain.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.Events;
using Catalog.Application.Features.Seats;
using Catalog.Application.Features.Sections;
using Catalog.Contracts;
using Catalog.Contracts.Requests;
using Catalog.Contracts.Responses;
using Catalog.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application;

/// <summary>
/// Represents the Catalog module in the application.
/// </summary>
public sealed class CatalogModule(ICatalogDbContext dbContext) : ICatalogModule
{
    public async Task<Result<EventPricingResponse>> GetEventPricingAsync(
        long eventId,
        IReadOnlyCollection<SeatPriceRequest>? seatPriceRequests,
        IReadOnlyCollection<GeneralAdmissionPriceRequest>? generalAdmissionPriceRequests,
        CancellationToken cancellationToken = default
    )
    {
        var @event = await dbContext
            .Events.Where(e => e.Id == eventId)
            .Select(e => new
            {
                e.Id,
                e.Status,
                e.Currency,
                e.VenueId,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (@event is null)
        {
            return EventErrors.NotFound(eventId);
        }

        if (@event.Status != EventStatus.Published)
        {
            return EventErrors.PricesUnavailable(@event.Id, @event.Status);
        }

        Result<List<SeatPriceResponse>> seatPrices = await GetSeatPricesAsync(
            @event.Id,
            @event.VenueId,
            seatPriceRequests ?? [],
            cancellationToken
        );
        if (seatPrices.IsFailure)
        {
            return seatPrices.Error;
        }

        Result<List<GeneralAdmissionPriceResponse>> admissionPrices =
            await GetGeneralAdmissionPricesAsync(
                @event.Id,
                @event.VenueId,
                generalAdmissionPriceRequests ?? [],
                cancellationToken
            );
        if (admissionPrices.IsFailure)
        {
            return admissionPrices.Error;
        }

        return new EventPricingResponse(
            @event.Id,
            @event.Currency,
            seatPrices.Value,
            admissionPrices.Value
        );
    }

    private async Task<Result<List<SeatPriceResponse>>> GetSeatPricesAsync(
        long eventId,
        long venueId,
        IReadOnlyCollection<SeatPriceRequest> requests,
        CancellationToken cancellationToken
    )
    {
        if (requests.Count == 0)
        {
            return new List<SeatPriceResponse>();
        }

        Dictionary<long, HashSet<long>> ticketTypesBySeat = requests
            .GroupBy(x => x.SeatId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.TicketTypeId).ToHashSet());

        long[] seatIds = [.. ticketTypesBySeat.Keys];

        Dictionary<long, long> zoneBySeat = await dbContext
            .Seats.Where(s => s.Row!.Section!.VenueId == venueId && seatIds.Contains(s.Id))
            .Select(s => new { s.Id, s.Row!.Section!.PricingZoneId })
            .ToDictionaryAsync(x => x.Id, x => x.PricingZoneId, cancellationToken);

        if (zoneBySeat.Count < seatIds.Length)
        {
            long[] missingSeatIds = [.. seatIds.Except(zoneBySeat.Keys)];
            return SeatErrors.NotFoundForEvent(eventId, missingSeatIds);
        }

        Dictionary<(long PricingZoneId, long TicketTypeId), decimal> priceMap =
            await LoadPriceMapAsync(
                eventId,
                zoneBySeat.Values,
                ticketTypesBySeat.Values.SelectMany(x => x),
                cancellationToken
            );

        return MapPrices(
            ticketTypesBySeat,
            zoneBySeat,
            priceMap,
            (seatId, ticketTypeId, price) => new SeatPriceResponse(seatId, ticketTypeId, price)
        );
    }

    private async Task<Result<List<GeneralAdmissionPriceResponse>>> GetGeneralAdmissionPricesAsync(
        long eventId,
        long venueId,
        IReadOnlyCollection<GeneralAdmissionPriceRequest> requests,
        CancellationToken cancellationToken
    )
    {
        if (requests.Count == 0)
        {
            return new List<GeneralAdmissionPriceResponse>();
        }

        Dictionary<long, HashSet<long>> ticketTypesBySection = requests
            .GroupBy(x => x.SectionId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.TicketTypeId).ToHashSet());

        long[] sectionIds = [.. ticketTypesBySection.Keys];

        Dictionary<long, long> zoneBySection = await dbContext
            .Sections.Where(s =>
                sectionIds.Contains(s.Id)
                && s.VenueId == venueId
                && s.Type == SectionType.GeneralAdmission
            )
            .Select(s => new { s.Id, s.PricingZoneId })
            .ToDictionaryAsync(x => x.Id, x => x.PricingZoneId, cancellationToken);

        if (zoneBySection.Count < sectionIds.Length)
        {
            long[] missingSectionIds = [.. sectionIds.Except(zoneBySection.Keys)];
            return SectionErrors.NotFoundForEvent(eventId, missingSectionIds);
        }

        Dictionary<(long PricingZoneId, long TicketTypeId), decimal> priceMap =
            await LoadPriceMapAsync(
                eventId,
                zoneBySection.Values,
                ticketTypesBySection.Values.SelectMany(x => x),
                cancellationToken
            );

        return MapPrices(
            ticketTypesBySection,
            zoneBySection,
            priceMap,
            (sectionId, ticketTypeId, price) =>
                new GeneralAdmissionPriceResponse(sectionId, ticketTypeId, price)
        );
    }

    private async Task<
        Dictionary<(long PricingZoneId, long TicketTypeId), decimal>
    > LoadPriceMapAsync(
        long eventId,
        IEnumerable<long> pricingZoneIds,
        IEnumerable<long> ticketTypeIds,
        CancellationToken cancellationToken
    )
    {
        long[] zones = [.. pricingZoneIds.Distinct()];
        long[] ticketTypes = [.. ticketTypeIds.Distinct()];

        return await dbContext
            .EventPrices.Where(ep =>
                ep.EventId == eventId
                && zones.Contains(ep.PricingZoneId)
                && ticketTypes.Contains(ep.TicketTypeId)
            )
            .Select(ep => new
            {
                ep.PricingZoneId,
                ep.TicketTypeId,
                ep.Amount,
            })
            .ToDictionaryAsync(
                x => (x.PricingZoneId, x.TicketTypeId),
                x => x.Amount,
                cancellationToken
            );
    }

    /// <summary>
    /// Maps each requested target and ticket type pair to a response.
    /// Pairs without a configured price are omitted.
    /// </summary>
    private static List<TResponse> MapPrices<TResponse>(
        Dictionary<long, HashSet<long>> ticketTypesByTarget,
        Dictionary<long, long> zoneByTarget,
        Dictionary<(long PricingZoneId, long TicketTypeId), decimal> priceMap,
        Func<long, long, decimal, TResponse> create
    )
    {
        var prices = new List<TResponse>();

        foreach ((long targetId, HashSet<long> ticketTypeIds) in ticketTypesByTarget)
        {
            long zoneId = zoneByTarget[targetId];
            foreach (long ticketTypeId in ticketTypeIds)
            {
                if (priceMap.TryGetValue((zoneId, ticketTypeId), out decimal price))
                {
                    prices.Add(create(targetId, ticketTypeId, price));
                }
            }
        }

        return prices;
    }
}
