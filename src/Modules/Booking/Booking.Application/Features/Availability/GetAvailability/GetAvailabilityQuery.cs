using BuildingBlocks.Application.Abstractions;

namespace Booking.Application.Features.Availability.GetAvailability;

public sealed record GetAvailabilityQuery(long EventId) : IQuery<AvailabilityDto>;
