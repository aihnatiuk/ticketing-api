using Booking.Domain.Enums;

namespace Booking.Application.Features.Availability.GetAvailability;

public sealed record AvailabilityDto(
    long EventId,
    IReadOnlyList<DedicatedSeatAvailabilityDto> DedicatedSeats,
    IReadOnlyList<GeneralSeatAvailabilityDto> GeneralAdmission
);

public sealed record DedicatedSeatAvailabilityDto(long SeatId, SeatStatus Status);

public sealed record GeneralSeatAvailabilityDto(long SectionId, int TotalSeats, int AvailableSeats);
