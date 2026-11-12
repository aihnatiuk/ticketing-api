using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.PricingZones.DeletePricingZone;

internal sealed class DeletePricingZoneCommandHandler(
    IPricingZoneRepository pricingZones,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<DeletePricingZoneCommand>
{
    public async Task<Result> HandleAsync(
        DeletePricingZoneCommand command,
        CancellationToken cancellationToken = default
    )
    {
        PricingZone? zone = await pricingZones.GetByIdAsync(command.Id, cancellationToken);
        if (zone is null || zone.VenueId != command.VenueId)
        {
            return PricingZoneErrors.NotFound(command.Id);
        }

        if (await pricingZones.HasSectionsAsync(zone.Id, cancellationToken))
        {
            return PricingZoneErrors.HasSections;
        }

        if (await pricingZones.HasEventPricesAsync(zone.Id, cancellationToken))
        {
            return PricingZoneErrors.HasEventPrices;
        }

        pricingZones.Remove(zone);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
