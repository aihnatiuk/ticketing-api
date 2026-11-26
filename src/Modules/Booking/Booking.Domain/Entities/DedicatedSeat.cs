using Booking.Domain.Enums;
using BuildingBlocks.Domain;

namespace Booking.Domain.Entities;

public sealed class DedicatedSeat : Entity<long>
{
    public long EventId { get; init; }

    public long SeatId { get; init; }

    public SeatStatus Status { get; private set; } = SeatStatus.Available;

    public long? OrderId { get; private set; }

    public void Reserve(long orderId)
    {
        Status = SeatStatus.Reserved;
        OrderId = orderId;
    }

    public void Release()
    {
        Status = SeatStatus.Available;
        OrderId = null;
    }
}
