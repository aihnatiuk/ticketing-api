using BuildingBlocks.Application.Abstractions;

namespace Catalog.Application.Features.Rows.CreateRows;

public sealed record CreateRowInput(string Label);

public sealed record CreateRowsCommand(long SectionId, IReadOnlyList<CreateRowInput> Rows)
    : ICommand<IReadOnlyList<long>>;
