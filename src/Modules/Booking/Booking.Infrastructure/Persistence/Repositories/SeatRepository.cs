using Booking.Application.Abstractions;
using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Persistence.Repositories;

internal sealed class SeatRepository(BookingDbContext context) : ISeatRepository
{
    public async Task<DedicatedSeat[]> GetDedicatedSeatsAsync(
        long eventId,
        IReadOnlyCollection<long> seatIds,
        CancellationToken cancellationToken = default
    )
    {
        if (seatIds.Count == 0)
        {
            return [];
        }

        return await context
            .DedicatedSeats.Where(s => s.EventId == eventId && seatIds.Contains(s.SeatId))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<GeneralSeat[]> GetGeneralAdmissionSeatsAsync(
        long eventId,
        IReadOnlyCollection<long> sectionIds,
        CancellationToken cancellationToken = default
    )
    {
        if (sectionIds.Count == 0)
        {
            return [];
        }

        return await context
            .GeneralSeats.AsNoTracking()
            .Where(s => s.EventId == eventId && sectionIds.Contains(s.SectionId))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<bool> TryReserveGeneralAdmissionAsync(
        long eventId,
        long sectionId,
        int quantity,
        CancellationToken cancellationToken = default
    )
    {
        int affected = await context
            .GeneralSeats.Where(s =>
                s.EventId == eventId && s.SectionId == sectionId && s.AvailableSeats >= quantity
            )
            .ExecuteUpdateAsync(
                set => set.SetProperty(s => s.AvailableSeats, s => s.AvailableSeats - quantity),
                cancellationToken
            );

        return affected == 1;
    }

    public async Task<bool> TryReleaseGeneralAdmissionAsync(
        long eventId,
        long sectionId,
        int quantity,
        CancellationToken cancellationToken = default
    )
    {
        int affected = await context
            .GeneralSeats.Where(s =>
                s.EventId == eventId
                && s.SectionId == sectionId
                && s.AvailableSeats + quantity <= s.TotalSeats
            )
            .ExecuteUpdateAsync(
                set => set.SetProperty(s => s.AvailableSeats, s => s.AvailableSeats + quantity),
                cancellationToken
            );

        return affected == 1;
    }
}
