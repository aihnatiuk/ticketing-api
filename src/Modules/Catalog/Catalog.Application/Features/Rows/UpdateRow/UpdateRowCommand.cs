using BuildingBlocks.Application.Abstractions;

namespace Catalog.Application.Features.Rows.UpdateRow;

public sealed record UpdateRowCommand(long Id, string Label) : ICommand;
