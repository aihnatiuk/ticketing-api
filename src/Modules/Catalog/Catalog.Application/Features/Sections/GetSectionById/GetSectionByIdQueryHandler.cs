using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.Sections.Common;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Sections.GetSectionById;

internal sealed class GetSectionByIdQueryHandler(ICatalogDbContext db)
    : IQueryHandler<GetSectionByIdQuery, SectionDto>
{
    public async Task<Result<SectionDto>> HandleAsync(
        GetSectionByIdQuery query,
        CancellationToken cancellationToken = default
    )
    {
        Section? section = await db
            .Sections.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == query.Id, cancellationToken);

        if (section is null)
        {
            return SectionErrors.NotFound(query.Id);
        }

        return SectionDto.FromDomain(section);
    }
}
