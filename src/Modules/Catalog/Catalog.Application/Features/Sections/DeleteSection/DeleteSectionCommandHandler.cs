using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.Features.Venues;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Sections.DeleteSection;

internal sealed class DeleteSectionCommandHandler(
    IVenueRepository venues,
    ISectionRepository sections,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<DeleteSectionCommand>
{
    public async Task<Result> Handle(
        DeleteSectionCommand command,
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

        sections.Remove(section);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
