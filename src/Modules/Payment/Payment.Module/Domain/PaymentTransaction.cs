using BuildingBlocks.Domain;
using BuildingBlocks.Domain.Exceptions;
using Payment.Module.Domain.Enums;

namespace Payment.Module.Domain;

public sealed class PaymentTransaction : Entity<long>
{
    private PaymentTransaction() { }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? ProcessedAt { get; private set; }

    public long OrderId { get; private set; }

    public string Currency { get; private set; } = null!;

    public decimal Amount { get; private set; }

    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;

    public static PaymentTransaction Create(long orderId, string currency, decimal amount) =>
        new()
        {
            OrderId = orderId,
            Currency = currency,
            Amount = amount,
            CreatedAt = DateTimeOffset.UtcNow,
        };

    public void UpdateAmount(string currency, decimal amount)
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new DomainInvariantException(
                $"Payment {Id} is {Status} and its amount can no longer be changed."
            );
        }

        Currency = currency;
        Amount = amount;
    }

    public void Complete()
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new DomainInvariantException(
                $"Payment {Id} is {Status} and cannot be completed."
            );
        }

        Status = PaymentStatus.Completed;
        ProcessedAt = DateTimeOffset.UtcNow;
    }
}
