using Catalog.Application.Features.TicketTypes.Common;
using Catalog.Domain.Enums;

namespace Catalog.Application.Features.Sections.GetSectionSeatMap;

public sealed record SectionSeatMapDto(
    long EventId,
    long SectionId,
    SectionType Type,
    string PricingZone,
    IReadOnlyList<SeatMapPriceDto> Pricing,
    int? Capacity,
    IReadOnlyList<SeatMapRowDto>? Rows
);

public sealed record SeatMapPriceDto(TicketTypeDto TicketType, decimal Amount);

public sealed record SeatMapRowDto(long RowId, string Label, IReadOnlyList<SeatMapSeatDto> Seats);

public sealed record SeatMapSeatDto(long Id, string Label);
