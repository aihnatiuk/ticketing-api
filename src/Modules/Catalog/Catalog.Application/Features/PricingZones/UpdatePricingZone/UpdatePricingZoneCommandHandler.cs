using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.PricingZones.UpdatePricingZone;

internal sealed class UpdatePricingZoneCommandHandler(
    IPricingZoneRepository pricingZones,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<UpdatePricingZoneCommand>
{
    public async Task<Result> HandleAsync(
        UpdatePricingZoneCommand command,
        CancellationToken cancellationToken = default
    )
    {
        PricingZone? zone = await pricingZones.GetByIdAsync(command.Id, cancellationToken);
        if (zone is null || zone.VenueId != command.VenueId)
        {
            return PricingZoneErrors.NotFound(command.Id);
        }

        if (zone.Label == command.Label)
        {
            return Result.Success();
        }

        if (
            await pricingZones.LabelExistsAsync(
                zone.VenueId,
                command.Label,
                zone.Id,
                cancellationToken
            )
        )
        {
            return PricingZoneErrors.LabelTaken(command.Label);
        }

        zone.Label = command.Label;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
