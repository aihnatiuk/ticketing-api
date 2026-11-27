using Booking.Application.Abstractions;
using Booking.Domain.Entities;
using Booking.Domain.Enums;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Results;
using Catalog.Contracts;
using Catalog.Contracts.Requests;
using Catalog.Contracts.Responses;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Features.Orders.Common.Services;

internal sealed class OrderingService(
    ISeatRepository seats,
    IUnitOfWork<BookingModule> unitOfWork,
    ICatalogModule catalogModule
) : IOrderingService
{
    public async Task<Result<PreparedOrderDetails>> PrepareOrderDetailsAsync(
        long eventId,
        OrderItemsRequestDto? request,
        CancellationToken cancellationToken = default
    )
    {
        IReadOnlyList<DedicatedSeatRequestDto> requestedSeats = request?.DedicatedSeats ?? [];
        IReadOnlyList<GeneralAdmissionRequestDto> requestedAdmissions =
            request?.GeneralAdmissions ?? [];

        DedicatedSeat[] loadedDedicatedSeats = [];
        if (requestedSeats.Count > 0)
        {
            Result<DedicatedSeat[]> seatsResult = await LoadRequestedDedicatedSeatsAsync(
                eventId,
                requestedSeats,
                cancellationToken
            );
            if (seatsResult.IsFailure)
            {
                return seatsResult.Error;
            }

            loadedDedicatedSeats = seatsResult.Value;
        }

        if (requestedAdmissions.Count > 0)
        {
            Result checkResult = await EnsureGeneralAdmissionSeatsCanBeOrderedAsync(
                eventId,
                requestedAdmissions,
                cancellationToken
            );
            if (checkResult.IsFailure)
            {
                return checkResult.Error;
            }
        }

        Result<EventPricingResponse> pricingResult = await catalogModule.GetEventPricingAsync(
            eventId,
            requestedSeats.Select(s => new SeatPriceRequest(s.SeatId, s.TicketTypeId)).ToArray(),
            requestedAdmissions
                .Select(a => new GeneralAdmissionPriceRequest(
                    a.SectionId,
                    a.TicketTypeId,
                    a.Quantity
                ))
                .ToArray(),
            cancellationToken
        );
        if (pricingResult.IsFailure)
        {
            return pricingResult.Error;
        }

        EventPricingResponse pricing = pricingResult.Value;
        Result<List<OrderItem>> itemsResult = BuildOrderItems(
            pricing,
            requestedSeats,
            requestedAdmissions
        );
        if (itemsResult.IsFailure)
        {
            return itemsResult.Error;
        }

        return new PreparedOrderDetails(
            eventId,
            pricing.Currency,
            OrderItems: itemsResult.Value,
            loadedDedicatedSeats,
            requestedAdmissions
        );
    }

    public async Task<Result> ReserveAsync(
        long orderId,
        PreparedOrderDetails orderDetails,
        CancellationToken cancellationToken = default
    )
    {
        foreach (DedicatedSeat seat in orderDetails.LoadedDedicatedSeats)
        {
            seat.Reserve(orderId);
        }

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return OrderErrors.SeatsUnavailable(
                orderDetails.LoadedDedicatedSeats.Select(s => s.SeatId).ToArray()
            );
        }

        // One update per section, always in the same order, so that concurrent orders
        // locking the same sections cannot deadlock each other.
        IEnumerable<(long SectionId, int Quantity)> reservations = orderDetails
            .RequestedGeneralAdmissions.GroupBy(a => a.SectionId)
            .Select(g => (SectionId: g.Key, Quantity: g.Sum(a => a.Quantity)))
            .OrderBy(r => r.SectionId);

        foreach ((long sectionId, int quantity) in reservations)
        {
            bool reserved = await seats.TryReserveGeneralAdmissionAsync(
                orderDetails.EventId,
                sectionId,
                quantity,
                cancellationToken
            );

            if (!reserved)
            {
                return OrderErrors.NotEnoughTickets([sectionId]);
            }
        }

        return Result.Success();
    }

    public async Task<Result> ReleaseReservationAsync(
        Order order,
        OrderItem item,
        CancellationToken cancellationToken = default
    )
    {
        if (item.Type == OrderItemType.DedicatedSeat)
        {
            DedicatedSeat[] loaded = await seats.GetDedicatedSeatsAsync(
                order.EventId,
                [item.CatalogRefId],
                cancellationToken
            );

            DedicatedSeat? seat = loaded.SingleOrDefault();
            if (seat is null || seat.Status != SeatStatus.Reserved || seat.OrderId != order.Id)
            {
                throw new InvalidOperationException(
                    $"Dedicated seat '{item.CatalogRefId}' is not reserved by order '{order.Id}'."
                );
            }

            seat.Release();

            return Result.Success();
        }

        bool released = await seats.TryReleaseGeneralAdmissionAsync(
            order.EventId,
            item.CatalogRefId,
            item.Quantity,
            cancellationToken
        );
        if (!released)
        {
            return OrderErrors.ConcurrentModification(order.Id);
        }

        return Result.Success();
    }

    public async Task ReleaseAllReservationsAsync(
        Order order,
        CancellationToken cancellationToken = default
    )
    {
        long[] seatIds =
        [
            .. order
                .OrderItems.Where(i => i.Type == OrderItemType.DedicatedSeat)
                .Select(i => i.CatalogRefId),
        ];

        DedicatedSeat[] loaded = await seats.GetDedicatedSeatsAsync(
            order.EventId,
            seatIds,
            cancellationToken
        );

        foreach (DedicatedSeat seat in loaded)
        {
            if (seat.Status != SeatStatus.Reserved || seat.OrderId != order.Id)
            {
                throw new InvalidOperationException(
                    $"Dedicated seat '{seat.SeatId}' is not reserved by order '{order.Id}'."
                );
            }

            seat.Release();
        }

        if (loaded.Length != seatIds.Length)
        {
            throw new InvalidOperationException(
                $"Order '{order.Id}' references dedicated seats that were not found."
            );
        }

        // Same per-section order as ReserveAsync, so concurrent updates cannot deadlock each other.
        IEnumerable<(long SectionId, int Quantity)> releases = order
            .OrderItems.Where(i => i.Type == OrderItemType.GeneralAdmission)
            .GroupBy(i => i.CatalogRefId)
            .Select(g => (SectionId: g.Key, Quantity: g.Sum(i => i.Quantity)))
            .OrderBy(r => r.SectionId);

        foreach ((long sectionId, int quantity) in releases)
        {
            bool released = await seats.TryReleaseGeneralAdmissionAsync(
                order.EventId,
                sectionId,
                quantity,
                cancellationToken
            );
            if (!released)
            {
                throw new InvalidOperationException(
                    $"General admission section '{sectionId}' could not take back {quantity} seats of order '{order.Id}'."
                );
            }
        }
    }

    private async Task<Result<DedicatedSeat[]>> LoadRequestedDedicatedSeatsAsync(
        long eventId,
        IReadOnlyList<DedicatedSeatRequestDto> requested,
        CancellationToken cancellationToken
    )
    {
        long[] seatIds = requested.Select(s => s.SeatId).Distinct().ToArray();
        DedicatedSeat[] loaded = await seats.GetDedicatedSeatsAsync(
            eventId,
            seatIds,
            cancellationToken
        );

        long[] missingSeats = seatIds.Except(loaded.Select(s => s.SeatId)).ToArray();
        if (missingSeats.Length > 0)
        {
            return OrderErrors.SeatsNotFound(missingSeats);
        }

        long[] unavailableSeats = loaded
            .Where(s => s.Status != SeatStatus.Available)
            .Select(s => s.SeatId)
            .ToArray();

        if (unavailableSeats.Length > 0)
        {
            return OrderErrors.SeatsUnavailable(unavailableSeats);
        }

        return loaded;
    }

    private async Task<Result> EnsureGeneralAdmissionSeatsCanBeOrderedAsync(
        long eventId,
        IReadOnlyList<GeneralAdmissionRequestDto> requested,
        CancellationToken cancellationToken
    )
    {
        Dictionary<long, int> quantityBySection = requested
            .GroupBy(a => a.SectionId)
            .ToDictionary(g => g.Key, g => g.Sum(a => a.Quantity));

        GeneralSeat[] loaded = await seats.GetGeneralAdmissionSeatsAsync(
            eventId,
            quantityBySection.Keys,
            cancellationToken
        );

        long[] missing = quantityBySection.Keys.Except(loaded.Select(s => s.SectionId)).ToArray();
        if (missing.Length > 0)
        {
            return OrderErrors.SectionsNotFound(missing);
        }

        long[] soldOutSections = loaded
            .Where(s => s.AvailableSeats < quantityBySection[s.SectionId])
            .Select(s => s.SectionId)
            .ToArray();
        if (soldOutSections.Length > 0)
        {
            return OrderErrors.NotEnoughTickets(soldOutSections);
        }

        return Result.Success();
    }

    private static Result<List<OrderItem>> BuildOrderItems(
        EventPricingResponse pricing,
        IReadOnlyList<DedicatedSeatRequestDto> requestedSeats,
        IReadOnlyList<GeneralAdmissionRequestDto> requestedAdmissions
    )
    {
        Dictionary<(long, long), decimal> seatPrices = pricing.SeatPrices.ToDictionary(
            p => (p.SeatId, p.TicketTypeId),
            p => p.Price
        );
        Dictionary<(long, long), decimal> admissionPrices =
            pricing.GeneralAdmissionPrices.ToDictionary(
                p => (p.SectionId, p.TicketTypeId),
                p => p.Price
            );

        List<OrderItem> items = [];
        List<object> unavailable = [];

        foreach (DedicatedSeatRequestDto seat in requestedSeats)
        {
            if (!seatPrices.TryGetValue((seat.SeatId, seat.TicketTypeId), out decimal price))
            {
                unavailable.Add(new { seat.SeatId, seat.TicketTypeId });
                continue;
            }

            items.Add(OrderItem.CreateDedicatedSeat(seat.SeatId, seat.TicketTypeId, price));
        }

        foreach (GeneralAdmissionRequestDto admission in requestedAdmissions)
        {
            if (
                !admissionPrices.TryGetValue(
                    (admission.SectionId, admission.TicketTypeId),
                    out decimal price
                )
            )
            {
                unavailable.Add(new { admission.SectionId, admission.TicketTypeId });
                continue;
            }

            items.Add(
                OrderItem.CreateGeneralAdmission(
                    admission.SectionId,
                    admission.TicketTypeId,
                    admission.Quantity,
                    price
                )
            );
        }

        // Catalog omits ticket types that have no price in a pricing zone, so a missing price in the response means
        // that the user asked for a ticket type which is not being sold there.
        if (unavailable.Count > 0)
        {
            return OrderErrors.TicketTypesNotAvailable(unavailable);
        }

        return items;
    }
}
