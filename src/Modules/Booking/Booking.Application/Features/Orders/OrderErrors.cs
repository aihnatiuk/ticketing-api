using BuildingBlocks.Domain.Results;

namespace Booking.Application.Features.Orders;

public static class OrderErrors
{
    public static Error NotFound(long orderId) =>
        Error.NotFound("Order.NotFound", $"Order with id: '{orderId}' was not found.");

    public static Error NoItemsRequested() =>
        Error.Validation("Order.NoItemsRequested", "At least one item must be requested.");

    public static Error NotPending(long orderId) =>
        Error.Conflict("Order.NotPending", $"Order with id: '{orderId}' is not pending.");

    public static Error NotPayable(long orderId) =>
        Error.Conflict(
            "Order.NotPayable",
            $"Order with id: '{orderId}' is not in a state that allows payment."
        );

    public static Error EmptyOrder(long orderId) =>
        Error.Validation("Order.Empty", $"Order with id: '{orderId}' has no items to pay for.");

    public static Error Expired(long orderId) =>
        Error.Conflict("Order.Expired", $"Order with id: '{orderId}' has expired.");

    public static Error ConcurrentModification(long orderId) =>
        Error.Conflict(
            "Order.ConcurrentModification",
            $"Order with id: '{orderId}' was modified concurrently. Retry the request."
        );

    public static Error SeatsNotFound(IReadOnlyCollection<long> seatIds) =>
        Error.NotFound(
            "Order.SeatsNotFound",
            $"Some seats were not found for this event.",
            seatIds
        );

    public static Error SeatsUnavailable(IReadOnlyCollection<long> seatIds) =>
        Error.Conflict("Order.SeatsUnavailable", $"Some seats are not available.", seatIds);

    public static Error SectionsNotFound(IReadOnlyCollection<long> sectionIds) =>
        Error.NotFound(
            "Order.SectionsNotFound",
            $"Some general admission sections were not found for this event.",
            sectionIds
        );

    public static Error TicketTypesNotAvailable(IReadOnlyCollection<object> items) =>
        Error.Validation(
            "Order.TicketTypesNotAvailable",
            "Some ticket types are not available for the requested seats or sections.",
            items
        );

    public static Error NotEnoughTickets(IReadOnlyCollection<long> sectionIds) =>
        Error.Conflict(
            "Order.NotEnoughTickets",
            "Not enough tickets left in general admission sections.",
            sectionIds
        );
}
