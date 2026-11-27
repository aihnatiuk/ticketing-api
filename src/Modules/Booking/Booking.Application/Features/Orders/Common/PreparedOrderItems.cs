using Booking.Domain.Entities;

namespace Booking.Application.Features.Orders.Common;

/// <summary>
/// Priced order items that passed the availability checks, together with the data needed to reserve them.
/// </summary>
/// <param name="EventId">Event the items belong to.</param>
/// <param name="Currency">Currency reported by Catalog for the event.</param>
/// <param name="OrderItems">Order items with frozen unit prices.</param>
/// <param name="LoadedDedicatedSeats">Tracked dedicated seats to reserve.</param>
/// <param name="RequestedGeneralAdmissions">Requested general admissions, to be reserved per section.</param>
public sealed record PreparedOrderDetails(
    long EventId,
    string Currency,
    IReadOnlyList<OrderItem> OrderItems,
    IReadOnlyList<DedicatedSeat> LoadedDedicatedSeats,
    IReadOnlyList<GeneralAdmissionRequestDto> RequestedGeneralAdmissions
);
