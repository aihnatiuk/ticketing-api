using BuildingBlocks.Domain;
using Payment.Module.Domain.Enums;

namespace Payment.Module.Domain;

public sealed class PaymentTransaction : Entity<long>
{
    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? ProcessedAt { get; set; }

    public long OrderId { get; set; }

    public required string Currency { get; set; }

    public decimal Amount { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
}
