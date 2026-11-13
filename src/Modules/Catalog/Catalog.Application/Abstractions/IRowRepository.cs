using BuildingBlocks.Application.Abstractions;
using Catalog.Domain.Entities;

namespace Catalog.Application.Abstractions;

public interface IRowRepository : IRepository<Row, long>
{
    Task<Row?> GetByIdWithSectionAsync(long id, CancellationToken cancellationToken = default);

    Task<bool> LabelExistsAsync(
        long sectionId,
        string label,
        long? excludeRowId = null,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<string>> GetExistingLabelsAsync(
        long sectionId,
        IReadOnlyCollection<string> labels,
        CancellationToken cancellationToken = default
    );

    Task RemoveBySectionAsync(long sectionId, CancellationToken cancellationToken = default);
}
