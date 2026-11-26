namespace Catalog.Contracts.Requests;

public sealed record SeatPriceRequest(long SeatId, long TicketTypeId);
