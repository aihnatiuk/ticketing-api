using BuildingBlocks.Application.Abstractions;
using Catalog.Domain.Enums;

namespace Catalog.Application.Features.Sections.CreateSections;

public sealed record CreateSectionInput(
    string Label,
    SectionType Type,
    int? Capacity,
    long PricingZoneId
);

public sealed record CreateSectionsCommand(long VenueId, IReadOnlyList<CreateSectionInput> Sections)
    : ICommand<IReadOnlyList<long>>;
