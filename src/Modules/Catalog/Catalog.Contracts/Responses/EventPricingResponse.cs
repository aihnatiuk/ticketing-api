namespace Catalog.Contracts.Responses;

public sealed record EventPricingResponse(
    long EventId,
    string Currency,
    IReadOnlyCollection<SeatPriceResponse> SeatPrices,
    IReadOnlyCollection<GeneralAdmissionPriceResponse> GeneralAdmissionPrices
);

public sealed record SeatPriceResponse(long SeatId, long TicketTypeId, decimal Price);

public sealed record GeneralAdmissionPriceResponse(
    long SectionId,
    long TicketTypeId,
    decimal Price
);
