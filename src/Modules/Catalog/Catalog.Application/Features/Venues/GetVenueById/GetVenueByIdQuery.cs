using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Features.Venues.Common;

namespace Catalog.Application.Features.Venues.GetVenueById;

public sealed record GetVenueByIdQuery(long Id) : IQuery<VenueDto>;
