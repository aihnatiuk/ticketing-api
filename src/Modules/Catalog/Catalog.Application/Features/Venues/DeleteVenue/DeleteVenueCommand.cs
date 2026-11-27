using BuildingBlocks.Application.Abstractions;

namespace Catalog.Application.Features.Venues.DeleteVenue;

public sealed record DeleteVenueCommand(long Id) : ICommand;
