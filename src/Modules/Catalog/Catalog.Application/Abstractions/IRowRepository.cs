using BuildingBlocks.Application.Abstractions;
using Catalog.Domain.Entities;

namespace Catalog.Application.Abstractions;

public interface IRowRepository : IRepository<Row, long>
{
    Task RemoveBySectionAsync(long sectionId, CancellationToken cancellationToken = default);
}
