using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Features.Venues.Common;

namespace Catalog.Application.Features.Venues.CreateVenue;

public sealed record CreateVenueCommand(string Name, AddressDto Address) : ICommand<long>;
