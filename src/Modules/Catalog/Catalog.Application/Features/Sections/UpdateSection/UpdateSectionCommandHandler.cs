using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.PricingZones;
using Catalog.Application.Features.Venues;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;

namespace Catalog.Application.Features.Sections.UpdateSection;

internal sealed class UpdateSectionCommandHandler(
    IVenueRepository venues,
    IPricingZoneRepository pricingZones,
    ISectionRepository sections,
    IRowRepository rows,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<UpdateSectionCommand>
{
    public async Task<Result> HandleAsync(
        UpdateSectionCommand command,
        CancellationToken cancellationToken = default
    )
    {
        Section? section = await sections.GetByIdAsync(command.Id, cancellationToken);
        if (section is null)
        {
            return SectionErrors.NotFound(command.Id);
        }

        if (await venues.HasEventsAsync(section.VenueId, cancellationToken))
        {
            return VenueErrors.LayoutLocked;
        }

        bool labelExists = await sections.LabelExistsAsync(
            section.VenueId,
            command.Label,
            section.Id,
            cancellationToken
        );
        if (labelExists)
        {
            return SectionErrors.LabelTaken(command.Label);
        }

        PricingZone? zone = await pricingZones.GetByIdAsync(
            command.PricingZoneId,
            cancellationToken
        );
        if (zone is null || zone.VenueId != section.VenueId)
        {
            return PricingZoneErrors.NotFound(command.PricingZoneId);
        }

        if (
            section.Type == SectionType.ReservedSeating
            && command.Type == SectionType.GeneralAdmission
        )
        {
            await rows.RemoveBySectionAsync(section.Id, cancellationToken);
        }

        section.Label = command.Label;
        section.PricingZoneId = command.PricingZoneId;
        section.ChangeLayout(command.Type, command.Capacity);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
