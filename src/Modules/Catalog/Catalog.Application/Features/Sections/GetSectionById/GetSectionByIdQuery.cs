using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Features.Sections.Common;

namespace Catalog.Application.Features.Sections.GetSectionById;

public sealed record GetSectionByIdQuery(long Id) : IQuery<SectionDto>;
