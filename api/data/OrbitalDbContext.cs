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
    public DbSet<Mission> Missions { get; set; }
    public DbSet<Rocket> Rockets { get; set; }
    public DbSet<SpaceStation> SpaceStations { get; set; }
}