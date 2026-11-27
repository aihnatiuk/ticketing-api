using BuildingBlocks.Infrastructure.Persistence;
using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;

namespace Catalog.Infrastructure.Persistence.Repositories;

internal sealed class EventRepository(CatalogDbContext context)
    : BaseRepository<Event, long>(context),
        IEventRepository { }
