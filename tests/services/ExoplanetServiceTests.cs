using Moq;
using Orbital.Api.Infrastructure;
using Orbital.Api.Models.Entities;
using Orbital.Api.Services;

namespace Orbital.Tests.Services;

public class ExoplanetServiceTests
{
    [Fact]
    public async Task GetAll_FiltersByMethodAndYearRange()
    {
        await using var context = TestDbContextFactory.Create();
        var redis = new Mock<IRedisService>();
        redis.Setup(item => item.GetAsync<List<Exoplanet>>(CacheKeys.ExoplanetData))
            .ReturnsAsync([
                new Exoplanet { PlanetName = "Kepler-1b", DiscoveryMethod = "Transit", DiscoveryYear = 2015 },
                new Exoplanet { PlanetName = "Kepler-2b", DiscoveryMethod = "Transit", DiscoveryYear = 2020 },
                new Exoplanet { PlanetName = "Proxima b", DiscoveryMethod = "Radial Velocity", DiscoveryYear = 2016 }
            ]);

        var result = await new ExoplanetService(context, redis.Object)
            .GetAll("transit", 2014, 2016);

        Assert.True(result.IsSuccess);
        var planet = Assert.Single(result.Value!);
        Assert.Equal("Kepler-1b", planet.PlanetName);
    }
}
