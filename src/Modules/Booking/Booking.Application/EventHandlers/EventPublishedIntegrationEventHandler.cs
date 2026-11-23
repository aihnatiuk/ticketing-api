using Booking.Application.Abstractions;
using Booking.Domain.Entities;
using BuildingBlocks.Application.Messaging;
using Catalog.Contracts.IntegrationEvents;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.EventHandlers;

/// <summary>
/// Creates the availability records of a newly published event.
/// </summary>
internal sealed class EventPublishedIntegrationEventHandler(IBookingDbContext db)
    : IIntegrationEventHandler<EventPublishedIntegrationEvent>
{
    public async Task HandleAsync(
        EventPublishedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default
    )
    {
        bool alreadySeeded =
            await db.DedicatedSeats.AnyAsync(
                s => s.EventId == integrationEvent.EventId,
                cancellationToken
            )
            || await db.GeneralSeats.AnyAsync(
                s => s.EventId == integrationEvent.EventId,
                cancellationToken
            );
        if (alreadySeeded)
        {
            return;
        }

        db.GeneralSeats.AddRange(
            integrationEvent.GeneralAdmissionSections.Select(section => new GeneralSeat
            {
                EventId = integrationEvent.EventId,
                SectionId = section.SectionId,
                TotalSeats = section.Capacity,
                AvailableSeats = section.Capacity,
            })
        );

        db.DedicatedSeats.AddRange(
            integrationEvent.SeatIds.Select(seatId => new DedicatedSeat
            {
                EventId = integrationEvent.EventId,
                SeatId = seatId,
            })
        );

        await db.SaveChangesAsync(cancellationToken);
    }
}
