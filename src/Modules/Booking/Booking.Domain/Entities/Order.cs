using Booking.Domain.Enums;
using BuildingBlocks.Domain;
using BuildingBlocks.Domain.Exceptions;

namespace Booking.Domain.Entities;

public sealed class Order : Entity<long>
{
    /// <summary>
    /// The number of failed payment attempts after which the order abandoned and its seats are released.
    /// </summary>
    public const int MaxPaymentAttempts = 3;

    public static readonly TimeSpan ExpirationTimeout = TimeSpan.FromMinutes(5);

    /// <summary>
    /// The time given to retry the payment after a failed attempt.
    /// </summary>
    public static readonly TimeSpan PaymentRetryTimeout = TimeSpan.FromMinutes(1);

    private readonly List<OrderItem> _orderItems = [];

    private Order() { }

    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// The expiration date of a draft order. If the order is not completed before this date,
    /// it will be automatically canceled and the reserved seats will be released.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; private set; }

    public Guid UserId { get; private set; }

    public long EventId { get; private set; }

    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    public string Currency { get; private set; } = null!;

    public decimal TotalPrice { get; private set; }

    public long? PaymentId { get; private set; }

    /// <summary>
    /// Concurrency token. Incremented on every change of the order or its items.
    /// </summary>
    public int Revision { get; private set; }

    public IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public static Order Create(
        Guid userId,
        long eventId,
        string currency,
        IEnumerable<OrderItem>? items
    )
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        Order order = new()
        {
            CreatedAt = now,
            ExpiresAt = now.Add(ExpirationTimeout),
            UserId = userId,
            EventId = eventId,
            Currency = currency,
        };

        if (items is not null)
        {
            order.AddItems(items);
        }

        return order;
    }

    /// <summary>
    /// Adds items to the order and recalculates the total price.
    /// </summary>
    /// <exception cref="DomainInvariantException">
    /// The order is not pending, or a dedicated seat is added more than once, whatever the ticket type is.
    /// </exception>
    public void AddItems(IEnumerable<OrderItem> items)
    {
        if (Status != OrderStatus.Pending)
        {
            throw new DomainInvariantException(
                $"Items cannot be added to an order with status '{Status}'."
            );
        }

        HashSet<long> seatIds = _orderItems
            .Where(i => i.Type == OrderItemType.DedicatedSeat)
            .Select(i => i.CatalogRefId)
            .ToHashSet();

        List<OrderItem> newItems = [.. items];
        foreach (OrderItem item in newItems)
        {
            if (item.Type == OrderItemType.DedicatedSeat && !seatIds.Add(item.CatalogRefId))
            {
                throw new DomainInvariantException(
                    $"Dedicated seat '{item.CatalogRefId}' cannot be added to the order more than once."
                );
            }
        }

        _orderItems.AddRange(newItems);
        OnOrderItemsChanged();
    }

    /// <summary>
    /// Removes an item from the order and recalculates the total price.
    /// </summary>
    /// <returns>True if the item was removed; false if the order has no such item.</returns>
    /// <exception cref="DomainInvariantException">
    /// The order is not pending.
    /// </exception>
    public bool RemoveItem(long itemId)
    {
        if (Status != OrderStatus.Pending)
        {
            throw new DomainInvariantException(
                $"Items cannot be removed from an order with status '{Status}'."
            );
        }

        OrderItem? item = _orderItems.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
        {
            return false;
        }

        _orderItems.Remove(item);
        OnOrderItemsChanged();

        return true;
    }

    /// <summary>
    /// Moves the order to payment processing and links it to the payment.
    /// </summary>
    /// <exception cref="DomainInvariantException">The order is not pending.</exception>
    public void StartPayment(long paymentId)
    {
        if (Status != OrderStatus.Pending)
        {
            throw new DomainInvariantException(
                $"Payment cannot be started for an order with status '{Status}'."
            );
        }

        PaymentId = paymentId;
        Status = OrderStatus.PaymentProcessing;
        Revision++;
    }

    /// <summary>
    /// Marks the order as paid.
    /// </summary>
    /// <exception cref="DomainInvariantException">The order is not awaiting payment.</exception>
    public void MarkPaid()
    {
        if (Status != OrderStatus.PaymentProcessing)
        {
            throw new DomainInvariantException(
                $"An order with status '{Status}' cannot be marked as paid."
            );
        }

        Status = OrderStatus.Paid;
        Revision++;
    }

    /// <summary>
    /// Records a failed payment attempt. While attempts remain, the order returns to pending with a short
    /// window to retry; once <see cref="MaxPaymentAttempts"/> is reached, the order is given up.
    /// </summary>
    /// <param name="failedAttempts">Number of failed payments of the order, including the one that just failed.</param>
    /// <returns>True if attempts are exhausted and the reserved seats must be released.</returns>
    /// <exception cref="DomainInvariantException">The order is not awaiting payment.</exception>
    public bool FailPayment(int failedAttempts)
    {
        if (Status != OrderStatus.PaymentProcessing)
        {
            throw new DomainInvariantException(
                $"Payment cannot fail for an order with status '{Status}'."
            );
        }

        PaymentId = null;
        Revision++;

        bool attemptsExhausted = failedAttempts >= MaxPaymentAttempts;
        if (attemptsExhausted)
        {
            Status = OrderStatus.PaymentFailed;
            return true;
        }

        Status = OrderStatus.Pending;
        if (ExpiresAt - DateTimeOffset.UtcNow < PaymentRetryTimeout)
        {
            // Extend reservation so that the user can safely retry payment up to MaxPaymentAttempts
            // without order being automatically expired and reservations released.
            ExpiresAt = ExpiresAt.Add(PaymentRetryTimeout);
        }

        return false;
    }

    public bool IsExpired() => ExpiresAt <= DateTimeOffset.UtcNow;

    private void OnOrderItemsChanged()
    {
        TotalPrice = _orderItems.Sum(i => i.UnitPrice * i.Quantity);
        Revision++;
    }
}
