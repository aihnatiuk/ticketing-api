using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Features.TicketTypes.Common;

namespace Catalog.Application.Features.TicketTypes.GetTicketTypes;

public sealed record GetTicketTypesQuery : IQuery<IReadOnlyList<TicketTypeDto>>;
