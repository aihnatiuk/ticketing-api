using BuildingBlocks.Application.Abstractions;

namespace Catalog.Application.Features.EventPrices.UpdateEventPrice;

public sealed record UpdateEventPriceCommand(long Id, decimal Amount) : ICommand;
