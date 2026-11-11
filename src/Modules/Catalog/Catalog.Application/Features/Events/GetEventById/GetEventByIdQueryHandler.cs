using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.Events.Common;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Events.GetEventById;

internal sealed class GetEventByIdQueryHandler(ICatalogDbContext db)
    : IQueryHandler<GetEventByIdQuery, EventDto>
{
    public async Task<Result<EventDto>> Handle(
        GetEventByIdQuery query,
        CancellationToken cancellationToken = default
    )
    {
        Event? entity = await db
            .Events.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == query.Id, cancellationToken);

        if (entity is null)
        {
            return EventErrors.NotFound(query.Id);
        }

        return EventDto.FromDomain(entity);
    }
}
