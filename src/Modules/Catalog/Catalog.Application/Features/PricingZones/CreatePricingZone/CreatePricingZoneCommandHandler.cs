using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.Venues;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.PricingZones.CreatePricingZone;

internal sealed class CreatePricingZoneCommandHandler(
    IVenueRepository venues,
    IPricingZoneRepository pricingZones,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<CreatePricingZoneCommand, long>
{
    public async Task<Result<long>> Handle(
        CreatePricingZoneCommand command,
        CancellationToken cancellationToken = default
    )
    {
        if (!await venues.ExistsAsync(command.VenueId, cancellationToken))
        {
            return VenueErrors.NotFound(command.VenueId);
        }

        if (
            await pricingZones.LabelExistsAsync(
                command.VenueId,
                command.Label,
                null,
                cancellationToken
            )
        )
        {
            return PricingZoneErrors.LabelTaken(command.Label);
        }

        var zone = new PricingZone { Label = command.Label, VenueId = command.VenueId };

        pricingZones.Add(zone);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return zone.Id;
    }
}
