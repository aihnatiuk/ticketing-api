using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Venues.CreateVenue;

internal sealed class CreateVenueCommandHandler(
    IVenueRepository venues,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<CreateVenueCommand, long>
{
    public async Task<Result<long>> HandleAsync(
        CreateVenueCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var venue = new Venue { Name = command.Name, Address = command.Address.ToDomain() };

        venues.Add(venue);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return venue.Id;
    }
}
