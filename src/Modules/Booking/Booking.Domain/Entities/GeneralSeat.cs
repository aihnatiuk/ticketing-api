using BuildingBlocks.Domain;

namespace Booking.Domain.Entities;

public sealed class GeneralSeat : Entity<long>
{
    public long EventId { get; init; }

    /// <summary>
    /// SectionId points to the Catalog section that allows general admission tickets.
    /// </summary>
    public long SectionId { get; init; }

    public int TotalSeats { get; init; }

    public int AvailableSeats { get; init; }
}
