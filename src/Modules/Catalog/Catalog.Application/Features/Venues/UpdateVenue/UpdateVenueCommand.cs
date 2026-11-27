using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Features.Venues.Common;

namespace Catalog.Application.Features.Venues.UpdateVenue;

public sealed record UpdateVenueCommand(long Id, string Name, AddressDto Address) : ICommand;
