using BuildingBlocks.Domain;

namespace Booking.Domain.Entities;

public sealed class GeneralSeat : Entity<long>
{
    public long EventId { get; set; }

    /// <summary>
    /// SectionId points to the Catalog section that allows general admission tickets.
    /// </summary>
    public long SectionId { get; set; }

    public int TotalSeats { get; set; }

    public int AvailableSeats { get; set; }
}
