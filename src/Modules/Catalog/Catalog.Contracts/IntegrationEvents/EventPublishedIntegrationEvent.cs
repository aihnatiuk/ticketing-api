using BuildingBlocks.Contracts;

namespace Catalog.Contracts.IntegrationEvents;

/// <summary>
/// Raised when an event has been published in the Catalog module.
/// Carries a complete snapshot regarding seat availability for the event.
/// </summary>
/// <param name="EventId">Id of the published event.</param>
/// <param name="SeatIds">Ids of all seats in the reserved-seating sections of the event's venue.</param>
/// <param name="GeneralAdmissionSections">Capacity of every general-admission section of the event's venue.</param>
[IntegrationEvent(ICatalogModule.Name, "event-published")]
public sealed record EventPublishedIntegrationEvent(
    long EventId,
    IReadOnlyList<long> SeatIds,
    IReadOnlyList<GeneralAdmissionSectionDto> GeneralAdmissionSections
);

/// <summary>
/// A general-admission section and the number of tickets that can be sold for it.
/// </summary>
/// <param name="SectionId">Id of the Catalog section.</param>
/// <param name="Capacity">Total number of admissions.</param>
public sealed record GeneralAdmissionSectionDto(long SectionId, int Capacity);
