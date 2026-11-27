using Booking.Domain.Enums;
using BuildingBlocks.Domain;

namespace Booking.Domain.Entities;

public sealed class DedicatedSeat : Entity<long>
{
    public long EventId { get; set; }

    public long SeatId { get; set; }

    public SeatStatus Status { get; set; } = SeatStatus.Available;

    public long? OrderId { get; set; }
}
