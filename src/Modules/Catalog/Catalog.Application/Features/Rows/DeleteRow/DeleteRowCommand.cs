using BuildingBlocks.Application.Abstractions;

namespace Catalog.Application.Features.Rows.DeleteRow;

public sealed record DeleteRowCommand(long Id) : ICommand;
