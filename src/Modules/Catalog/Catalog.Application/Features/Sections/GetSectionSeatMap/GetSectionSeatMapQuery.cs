using BuildingBlocks.Application.Abstractions;

namespace Catalog.Application.Features.Sections.GetSectionSeatMap;

public sealed record GetSectionSeatMapQuery(long EventId, long SectionId)
    : IQuery<SectionSeatMapDto>;
