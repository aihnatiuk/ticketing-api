using Booking.Domain.Enums;
using BuildingBlocks.Domain;

namespace Booking.Domain.Entities;

public sealed class Order : Entity<long>
{
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// The expiration date of a draft order. If the order is not completed before this date,
    /// it will be automatically canceled and the reserved seats will be released.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

    public Guid UserId { get; set; }

    public long EventId { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public required string Currency { get; set; }

    public decimal TotalPrice { get; set; }

    public long? PaymentId { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = [];
}
