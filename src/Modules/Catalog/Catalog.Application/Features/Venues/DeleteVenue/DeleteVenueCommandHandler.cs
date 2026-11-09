using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Venues.DeleteVenue;

internal sealed class DeleteVenueCommandHandler(
    IVenueRepository venues,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<DeleteVenueCommand>
{
    public async Task<Result> Handle(
        DeleteVenueCommand command,
        CancellationToken cancellationToken = default
    )
    {
        Venue? venue = await venues.GetByIdAsync(command.Id, cancellationToken);
        if (venue is null)
        {
            return VenueErrors.NotFound(command.Id);
        }

        if (await venues.HasEventsAsync(command.Id, cancellationToken))
        {
            return VenueErrors.HasEvents;
        }

        venues.Remove(venue);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
