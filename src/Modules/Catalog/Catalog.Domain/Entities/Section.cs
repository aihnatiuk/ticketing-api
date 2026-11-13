using BuildingBlocks.Domain;
using BuildingBlocks.Domain.Exceptions;
using Catalog.Domain.Enums;

namespace Catalog.Domain.Entities;

public sealed class Section : Entity<long>
{
    private Section() { }

    public required string Label { get; set; }

    public SectionType Type { get; private set; }

    /// <summary>
    /// General admission capacity if the <see cref="Type"/> is set to <see cref="SectionType.GeneralAdmission"/>, null otherwise.
    /// </summary>
    public int? Capacity { get; private set; }

    public required long VenueId { get; set; }

    public Venue? Venue { get; set; }

    public ICollection<Row> Rows { get; set; } = [];

    public required long PricingZoneId { get; set; }

    public PricingZone? PricingZone { get; set; }

    public static Section Create(
        string label,
        SectionType type,
        int? capacity,
        long venueId,
        long pricingZoneId
    )
    {
        EnsureValidLayout(type, capacity);

        return new Section
        {
            Label = label,
            Type = type,
            Capacity = capacity,
            VenueId = venueId,
            PricingZoneId = pricingZoneId,
        };
    }

    public static bool IsValidLayout(SectionType type, int? capacity) =>
        (type == SectionType.GeneralAdmission && capacity > 0)
        || (type != SectionType.GeneralAdmission && capacity is null);

    private static void EnsureValidLayout(SectionType type, int? capacity)
    {
        if (!IsValidLayout(type, capacity))
        {
            throw new DomainInvariantException(
                $"Unacceptable layout. Type '{type}' cannot have a capacity of '{capacity?.ToString() ?? "null"}'. "
                    + "General Admission requires a capacity; other types must be null."
            );
        }
    }

    public void ChangeLayout(SectionType type, int? capacity)
    {
        EnsureValidLayout(type, capacity);
        Type = type;
        Capacity = capacity;
    }
}
