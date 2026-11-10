using BuildingBlocks.Domain;
using Catalog.Domain.Enums;

namespace Catalog.Domain.Entities;

public sealed class Section : Entity<long>
{
    public required string Label { get; set; }

    public required SectionType Type { get; set; }

    /// <summary>
    /// General admission capacity if the <see cref="Type"/> is set to <see cref="SectionType.GeneralAdmission"/>, null otherwise.
    /// </summary>
    public int? Capacity { get; set; }

    public required long VenueId { get; set; }

    public Venue? Venue { get; set; }

    public ICollection<Row> Rows { get; set; } = [];

    public required long PricingZoneId { get; set; }

    public PricingZone? PricingZone { get; set; }
}
