using Microsoft.EntityFrameworkCore;
namespace Orbital.Api.Data;

public class OrbitalDbContext : DbContext
{
    public OrbitalDbContext(DbContextOptions<OrbitalDbContext> options)
        : base(options)
    {
    }

    // DbSet<T> properties go here in Phase 2, once entities exist
}