namespace Booking.Application.Features.Orders.Common;

public sealed record OrderItemsRequestDto(
    IReadOnlyList<DedicatedSeatRequestDto> DedicatedSeats,
    IReadOnlyList<GeneralAdmissionRequestDto> GeneralAdmissions
);

public sealed record DedicatedSeatRequestDto(long SeatId, long TicketTypeId);

public sealed record GeneralAdmissionRequestDto(long SectionId, long TicketTypeId, int Quantity);
