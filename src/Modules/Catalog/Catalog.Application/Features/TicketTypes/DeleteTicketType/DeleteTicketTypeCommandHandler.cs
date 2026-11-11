using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.TicketTypes.DeleteTicketType;

internal sealed class DeleteTicketTypeCommandHandler(
    ITicketTypeRepository ticketTypes,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<DeleteTicketTypeCommand>
{
    public async Task<Result> Handle(
        DeleteTicketTypeCommand command,
        CancellationToken cancellationToken = default
    )
    {
        TicketType? ticketType = await ticketTypes.GetByIdAsync(command.Id, cancellationToken);
        if (ticketType is null)
        {
            return TicketTypeErrors.NotFound(command.Id);
        }

        if (await ticketTypes.HasEventPricesAsync(ticketType.Id, cancellationToken))
        {
            return TicketTypeErrors.HasEventPrices;
        }

        ticketTypes.Remove(ticketType);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
