using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.TicketTypes.Common;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.TicketTypes.GetTicketTypes;

internal sealed class GetTicketTypesQueryHandler(ICatalogDbContext db)
    : IQueryHandler<GetTicketTypesQuery, IReadOnlyList<TicketTypeDto>>
{
    public async Task<Result<IReadOnlyList<TicketTypeDto>>> Handle(
        GetTicketTypesQuery query,
        CancellationToken cancellationToken = default
    )
    {
        List<TicketType> ticketTypes = await db
            .TicketTypes.AsNoTracking()
            .OrderBy(t => t.Label)
            .ToListAsync(cancellationToken);

        IReadOnlyList<TicketTypeDto> items = [.. ticketTypes.Select(TicketTypeDto.FromDomain)];

        return Result<IReadOnlyList<TicketTypeDto>>.Success(items);
    }
}
