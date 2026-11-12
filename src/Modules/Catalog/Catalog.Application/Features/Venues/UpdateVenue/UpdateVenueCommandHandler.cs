using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Venues.UpdateVenue;

internal sealed class UpdateVenueCommandHandler(
    IVenueRepository venues,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<UpdateVenueCommand>
{
    public async Task<Result> HandleAsync(
        UpdateVenueCommand command,
        CancellationToken cancellationToken = default
    )
    {
        Venue? venue = await venues.GetByIdAsync(command.Id, cancellationToken);
        if (venue is null)
        {
            return VenueErrors.NotFound(command.Id);
        }

        venue.Name = command.Name;
        venue.Address = command.Address.ToDomain();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
