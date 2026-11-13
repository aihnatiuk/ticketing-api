using Catalog.Domain.Entities;
using Catalog.Domain.Enums;

namespace Catalog.Application.Features.Sections.Common;

public sealed record SectionDto(
    long Id,
    long VenueId,
    string Label,
    SectionType Type,
    long PricingZoneId
)
{
    internal static SectionDto FromDomain(Section section) =>
        new(section.Id, section.VenueId, section.Label, section.Type, section.PricingZoneId);
}
