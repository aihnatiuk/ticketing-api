using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Features.TicketTypes.Common;

namespace Catalog.Application.Features.TicketTypes.GetTicketTypeById;

public sealed record GetTicketTypeByIdQuery(long Id) : IQuery<TicketTypeDto>;
