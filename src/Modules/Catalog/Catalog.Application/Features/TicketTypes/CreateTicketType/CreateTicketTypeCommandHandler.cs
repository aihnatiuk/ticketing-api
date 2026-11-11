using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.TicketTypes.CreateTicketType;

internal sealed class CreateTicketTypeCommandHandler(
    ITicketTypeRepository ticketTypes,
    IUnitOfWork<CatalogModule> unitOfWork
) : ICommandHandler<CreateTicketTypeCommand, long>
{
    public async Task<Result<long>> Handle(
        CreateTicketTypeCommand command,
        CancellationToken cancellationToken = default
    )
    {
        if (await ticketTypes.LabelExistsAsync(command.Label, null, cancellationToken))
        {
            return TicketTypeErrors.LabelTaken(command.Label);
        }

        var ticketType = new TicketType { Label = command.Label };

        ticketTypes.Add(ticketType);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ticketType.Id;
    }
}
