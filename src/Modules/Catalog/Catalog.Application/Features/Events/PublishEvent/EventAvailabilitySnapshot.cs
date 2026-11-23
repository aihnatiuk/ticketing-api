namespace Catalog.Application.Features.Events.PublishEvent;

/// <summary>
/// What can be sold for an event: the individual seats of its reserved-seating sections
/// and the capacity of its general-admission sections.
/// </summary>
/// <param name="SeatIds">Ids of all seats in the reserved-seating sections.</param>
/// <param name="GeneralAdmissionSections">Capacity of every general-admission section.</param>
internal sealed record EventAvailabilitySnapshot(
    IReadOnlyList<long> SeatIds,
    IReadOnlyList<GeneralAdmissionAvailability> GeneralAdmissionSections
)
{
    public bool IsEmpty => SeatIds.Count == 0 && GeneralAdmissionSections.Count == 0;
}

/// <summary>
/// A general-admission section and the number of admissions available in it.
/// </summary>
/// <param name="SectionId">Id of the section.</param>
/// <param name="Capacity">Total number of admissions.</param>
internal sealed record GeneralAdmissionAvailability(long SectionId, int Capacity);
