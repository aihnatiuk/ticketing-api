using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Features.Events.Common;

namespace Catalog.Application.Features.Events.GetEventById;

public sealed record GetEventByIdQuery(long Id) : IQuery<EventDto>;
