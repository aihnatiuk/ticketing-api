using BuildingBlocks.Application.Results;

namespace Booking.Application.Features.Availability;

public static class AvailabilityErrors
{
    public static Error EventNotFound(long eventId) =>
        Error.NotFound(
            "Availability.EventNotFound",
            $"Availability for event with id {eventId} was not found."
        );
}
