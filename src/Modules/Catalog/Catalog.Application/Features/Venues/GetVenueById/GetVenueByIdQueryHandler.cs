using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.Venues.Common;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Venues.GetVenueById;

internal sealed class GetVenueByIdQueryHandler(ICatalogDbContext db)
    : IQueryHandler<GetVenueByIdQuery, VenueDto>
{
    public async Task<Result<VenueDto>> Handle(
        GetVenueByIdQuery query,
        CancellationToken cancellationToken = default
    )
    {
        Venue? venue = await db
            .Venues.AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == query.Id, cancellationToken);

        if (venue is null)
        {
            return VenueErrors.NotFound(query.Id);
        }

        return VenueDto.FromDomain(venue);
    }
}
