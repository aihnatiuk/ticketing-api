using BuildingBlocks.Application.Abstractions;
using Catalog.Domain.Enums;

namespace Catalog.Application.Features.Sections.UpdateSection;

public sealed record UpdateSectionCommand(
    long Id,
    string Label,
    SectionType Type,
    int? Capacity,
    long PricingZoneId
) : ICommand;
