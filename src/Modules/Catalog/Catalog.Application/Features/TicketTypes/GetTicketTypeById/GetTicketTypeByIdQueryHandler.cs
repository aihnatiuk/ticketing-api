using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.TicketTypes.Common;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.TicketTypes.GetTicketTypeById;

internal sealed class GetTicketTypeByIdQueryHandler(ICatalogDbContext db)
    : IQueryHandler<GetTicketTypeByIdQuery, TicketTypeDto>
{
    public async Task<Result<TicketTypeDto>> Handle(
        GetTicketTypeByIdQuery query,
        CancellationToken cancellationToken = default
    )
    {
        TicketType? ticketType = await db
            .TicketTypes.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == query.Id, cancellationToken);

        if (ticketType is null)
        {
            return TicketTypeErrors.NotFound(query.Id);
        }

        return TicketTypeDto.FromDomain(ticketType);
    }
}
