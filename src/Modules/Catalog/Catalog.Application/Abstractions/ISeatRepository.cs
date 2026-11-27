using BuildingBlocks.Application.Abstractions;
using Catalog.Domain.Entities;

namespace Catalog.Application.Abstractions;

public interface ISeatRepository : IRepository<Seat, long>
{
    Task<Seat?> GetByIdWithRowAndSectionAsync(
        long id,
        CancellationToken cancellationToken = default
    );

    Task<bool> LabelExistsAsync(
        long rowId,
        string label,
        long? excludeSeatId = null,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<string>> GetExistingLabelsAsync(
        long rowId,
        IReadOnlyCollection<string> labels,
        CancellationToken cancellationToken = default
    );
}
