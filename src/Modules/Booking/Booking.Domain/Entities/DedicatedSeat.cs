using Booking.Domain.Enums;
using BuildingBlocks.Domain;
using BuildingBlocks.Domain.Exceptions;

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

    /// <summary>
    /// Marks a reserved seat as sold once the order it belongs to is paid.
    /// </summary>
    /// <exception cref="DomainInvariantException">The seat is not reserved.</exception>
    public void Sell()
    {
        if (Status != SeatStatus.Reserved)
        {
            throw new DomainInvariantException($"Seat '{SeatId}' is {Status} and cannot be sold.");
        }

        Status = SeatStatus.Sold;
    }
}
