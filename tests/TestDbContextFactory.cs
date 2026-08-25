using Microsoft.EntityFrameworkCore;
using Orbital.Api.Data;

namespace Orbital.Tests;

internal static class TestDbContextFactory
{
    public static OrbitalDbContext Create()
    {
        var options = new DbContextOptionsBuilder<OrbitalDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new OrbitalDbContext(options);
    }
}
