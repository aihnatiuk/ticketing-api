using Booking.Domain.Enums;
using BuildingBlocks.Domain;

namespace Booking.Domain.Entities;

public sealed class OrderItem : Entity<long>
{
    /// <summary>
    /// The order id that this item belongs to.
    /// </summary>
    public long OrderId { get; set; }

    public Order? Order { get; set; }

    /// <summary>
    /// The Catalog entity id that this order item is associated with.
    /// For dedicated seats, this will be the seat id.
    /// For general admission tickets, this will be the area id (e.g., a section).
    /// </summary>
    public long CatalogRefId { get; set; }

    /// <summary>
    /// The type of the order item.
    /// </summary>
    public OrderItemType Type { get; set; }

    /// <summary>
    /// The ticket type id from the Catalog that this order item is associated with. E.g., it can be a VIP ticket, Regular ticket, etc.
    /// </summary>
    public long TicketTypeId { get; set; }

    /// <summary>
    /// The quantity of tickets for this order item. For dedicated seats, this will always be 1. For general seats, this can be more than 1.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// The price of a single ticket for this order item. For dedicated seats, this will be the price of that specific seat.
    /// For general admission, this will be the price of the ticket type associated with this order item.
    /// </summary>
    public decimal UnitPrice { get; set; }
}
