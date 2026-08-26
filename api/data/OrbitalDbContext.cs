using Microsoft.EntityFrameworkCore;
using Orbital.Api.Models.Entities;
namespace Orbital.Api.Data;

public class OrbitalDbContext : DbContext
{
    public OrbitalDbContext(DbContextOptions<OrbitalDbContext> options)
        : base(options)
    {
    }
    public DbSet<ApodEntry> ApodEntries { get; set; }
    public DbSet<Asteroid> Asteroids { get; set; }
    public DbSet<Astronaut> Astronauts { get; set; }
    public DbSet<Exoplanet> Exoplanets { get; set; }
    public DbSet<Launch> Launches { get; set; }
    public DbSet<Mission> Missions { get; set; }
    public DbSet<Rocket> Rockets { get; set; }
    public DbSet<SpaceStation> SpaceStations { get; set; }
    public DbSet<CatalogSyncState> CatalogSyncStates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Astronaut>().HasIndex(entity => entity.SourceId);
        modelBuilder.Entity<Astronaut>().HasIndex(entity => entity.Name);
        modelBuilder.Entity<Rocket>().HasIndex(entity => new { entity.Name, entity.Variant });
        modelBuilder.Entity<Rocket>().HasIndex(entity => entity.SourceId);
        modelBuilder.Entity<Mission>().HasIndex(entity => entity.Name);
        modelBuilder.Entity<Mission>().HasIndex(entity => entity.SourceId);
        modelBuilder.Entity<Launch>().HasIndex(entity => entity.ExternalId);
        modelBuilder.Entity<Launch>().HasIndex(entity => entity.Net);
        modelBuilder.Entity<CatalogSyncState>().HasIndex(entity => entity.Catalog).IsUnique();
    }
}
