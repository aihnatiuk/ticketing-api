using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.PricingZones;
using Catalog.Application.Features.Venues;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Sections.CreateSections;

internal sealed class CreateSectionsCommandHandler(
    IVenueRepository venues,
    IPricingZoneRepository pricingZones,
    ISectionRepository sections,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<CreateSectionsCommand, IReadOnlyList<long>>
{
    public async Task<Result<IReadOnlyList<long>>> HandleAsync(
        CreateSectionsCommand command,
        CancellationToken cancellationToken = default
    )
    {
        if (!await venues.ExistsAsync(command.VenueId, cancellationToken))
        {
            return VenueErrors.NotFound(command.VenueId);
        }

        if (await venues.HasEventsAsync(command.VenueId, cancellationToken))
        {
            return VenueErrors.LayoutLocked;
        }

        IReadOnlyList<string> existingLabels = await sections.GetExistingLabelsAsync(
            command.VenueId,
            [.. command.Sections.Select(input => input.Label)],
            cancellationToken
        );

        if (existingLabels.Count > 0)
        {
            return SectionErrors.LabelTaken(existingLabels[0]);
        }

        long[] zoneIds = [.. command.Sections.Select(s => s.PricingZoneId).Distinct()];
        PricingZone[] foundZones = await pricingZones.GetByIdsAsync(zoneIds, cancellationToken);
        if (zoneIds.Length > foundZones.Length)
        {
            return PricingZoneErrors.NotFound(
                zoneIds.First(id => !foundZones.Any(z => z.Id == id))
            );
        }

        PricingZone? invalidZone = foundZones.FirstOrDefault(z => z.VenueId != command.VenueId);
        if (invalidZone is not null)
        {
            return PricingZoneErrors.VenueMismatch(invalidZone.Id, command.VenueId);
        }

        var createdSections = new List<Section>(command.Sections.Count);
        foreach (CreateSectionInput input in command.Sections)
        {
            Section section = Section.Create(
                input.Label,
                input.Type,
                input.Capacity,
                command.VenueId,
                input.PricingZoneId
            );
            sections.Add(section);
            createdSections.Add(section);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<IReadOnlyList<long>>.Success([
            .. createdSections.Select(section => section.Id),
        ]);
    }
}
