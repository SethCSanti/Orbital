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
}