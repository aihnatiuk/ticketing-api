using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.TicketTypes.UpdateTicketType;

internal sealed class UpdateTicketTypeCommandHandler(
    ITicketTypeRepository ticketTypes,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<UpdateTicketTypeCommand>
{
    public async Task<Result> Handle(
        UpdateTicketTypeCommand command,
        CancellationToken cancellationToken = default
    )
    {
        TicketType? ticketType = await ticketTypes.GetByIdAsync(command.Id, cancellationToken);
        if (ticketType is null)
        {
            return TicketTypeErrors.NotFound(command.Id);
        }

        if (await ticketTypes.LabelExistsAsync(command.Label, ticketType.Id, cancellationToken))
        {
            return TicketTypeErrors.LabelTaken(command.Label);
        }

        ticketType.Label = command.Label;
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
