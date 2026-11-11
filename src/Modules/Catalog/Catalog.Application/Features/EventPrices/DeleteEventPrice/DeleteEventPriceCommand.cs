using BuildingBlocks.Application.Abstractions;

namespace Catalog.Application.Features.EventPrices.DeleteEventPrice;

public sealed record DeleteEventPriceCommand(long Id) : ICommand;
