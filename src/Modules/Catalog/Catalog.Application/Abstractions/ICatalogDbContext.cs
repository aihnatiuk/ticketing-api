using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Abstractions;

public interface ICatalogDbContext
{
    DbSet<Venue> Venues { get; }

    DbSet<Section> Sections { get; }

    DbSet<Row> Rows { get; }

    DbSet<Seat> Seats { get; }

    DbSet<PricingZone> PricingZones { get; }

    DbSet<TicketType> TicketTypes { get; }

    DbSet<Event> Events { get; }

    DbSet<EventPrice> EventPrices { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
