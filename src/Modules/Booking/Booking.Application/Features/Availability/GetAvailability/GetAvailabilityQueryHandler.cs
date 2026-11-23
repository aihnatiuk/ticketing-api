using Booking.Application.Abstractions;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Features.Availability.GetAvailability;

internal sealed class GetAvailabilityQueryHandler(IBookingDbContext db)
    : IQueryHandler<GetAvailabilityQuery, AvailabilityDto>
{
    public async Task<Result<AvailabilityDto>> HandleAsync(
        GetAvailabilityQuery query,
        CancellationToken cancellationToken = default
    )
    {
        List<DedicatedSeatAvailabilityDto> dedicatedSeats = await db
            .DedicatedSeats.AsNoTracking()
            .Where(s => s.EventId == query.EventId)
            .OrderBy(s => s.SeatId)
            .Select(s => new DedicatedSeatAvailabilityDto(s.SeatId, s.Status))
            .ToListAsync(cancellationToken);

        List<GeneralSeatAvailabilityDto> generalSeats = await db
            .GeneralSeats.AsNoTracking()
            .Where(s => s.EventId == query.EventId)
            .OrderBy(s => s.SectionId)
            .Select(s => new GeneralSeatAvailabilityDto(
                s.SectionId,
                s.TotalSeats,
                s.AvailableSeats
            ))
            .ToListAsync(cancellationToken);

        if (dedicatedSeats.Count == 0 && generalSeats.Count == 0)
        {
            return AvailabilityErrors.EventNotFound(query.EventId);
        }

        return new AvailabilityDto(query.EventId, dedicatedSeats, generalSeats);
    }
}
