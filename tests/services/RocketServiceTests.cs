using Moq;
using Orbital.Api.Infrastructure;
using Orbital.Api.Models.Entities;
using Orbital.Api.Services;

namespace Orbital.Tests.Services;

public class RocketServiceTests
{
    [Fact]
    public async Task Compare_ReturnsRequestedRocketsCaseInsensitively()
    {
        await using var context = TestDbContextFactory.Create();
        var redis = new Mock<IRedisService>();
        redis.Setup(item => item.GetAsync<List<Rocket>>(CacheKeys.RocketData))
            .ReturnsAsync([
                new Rocket { Name = "Falcon 9", Variant = "Block 5" },
                new Rocket { Name = "Saturn V", Variant = "" }
            ]);

        var result = await new RocketService(context, redis.Object)
            .Compare(["FALCON 9"]);

        Assert.True(result.IsSuccess);
        var rocket = Assert.Single(result.Value!);
        Assert.Equal("Falcon 9", rocket.Name);
    }

    [Fact]
    public async Task GetByName_ReturnsFailureWhenMissing()
    {
        await using var context = TestDbContextFactory.Create();
        var redis = new Mock<IRedisService>();
        redis.Setup(item => item.GetAsync<List<Rocket>>(CacheKeys.RocketData))
            .ReturnsAsync(new List<Rocket>());

        var result = await new RocketService(context, redis.Object).GetByName("Unknown");

        Assert.False(result.IsSuccess);
        Assert.Contains("Unknown", result.Error);
    }
}
