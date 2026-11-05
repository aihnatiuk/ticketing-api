using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence;

public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options)
    : DbContext(options)
{
    public const string Schema = "catalog";

    public DbSet<Venue> Venues => Set<Venue>();

    public DbSet<Section> Sections => Set<Section>();

    public DbSet<Row> Rows => Set<Row>();

    public DbSet<Seat> Seats => Set<Seat>();

    public DbSet<PricingZone> PricingZones => Set<PricingZone>();

    public DbSet<TicketType> TicketTypes => Set<TicketType>();

    public DbSet<Event> Events => Set<Event>();

    public DbSet<EventPrice> EventPrices => Set<EventPrice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
    }
}
