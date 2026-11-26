using Booking.Domain.Entities;

namespace Booking.Application.Abstractions;

/// <summary>
/// Access to the seat availability records of an event, both dedicated and general admission.
/// </summary>
public interface ISeatRepository
{
    /// <summary>
    /// Loads the requested dedicated seats of an event as tracked entities, so that they can be reserved
    /// and the Status concurrency token verified on save.
    /// </summary>
    Task<DedicatedSeat[]> GetDedicatedSeatsAsync(
        long eventId,
        IReadOnlyCollection<long> seatIds,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Loads the general admission records of an event without tracking. Used only for fail-fast checks.
    /// </summary>
    Task<GeneralSeat[]> GetGeneralAdmissionSeatsAsync(
        long eventId,
        IReadOnlyCollection<long> sectionIds,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Atomically takes the quantity from the section's available seats.
    /// </summary>
    /// <returns>False if there are not enough available seats; nothing is changed in that case.</returns>
    Task<bool> TryReserveGeneralAdmissionAsync(
        long eventId,
        long sectionId,
        int quantity,
        CancellationToken cancellationToken = default
    );
}
