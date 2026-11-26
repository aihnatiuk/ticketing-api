namespace Catalog.Contracts.Requests;

public sealed record GeneralAdmissionPriceRequest(long SectionId, long TicketTypeId, int Quantity);
