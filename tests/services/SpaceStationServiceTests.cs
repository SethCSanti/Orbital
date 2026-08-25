using Moq;
using Orbital.Api.Infrastructure;
using Orbital.Api.Models.Entities;
using Orbital.Api.Services;

namespace Orbital.Tests.Services;

public class SpaceStationServiceTests
{
    [Fact]
    public async Task GetById_ReturnsMatchingCachedStation()
    {
        await using var context = TestDbContextFactory.Create();
        var redis = new Mock<IRedisService>();
        redis.Setup(item => item.GetAsync<List<SpaceStation>>(CacheKeys.SpaceStationData))
            .ReturnsAsync([
                new SpaceStation { Id = 3, Name = "ISS" },
                new SpaceStation { Id = 4, Name = "Tiangong" }
            ]);

        var result = await new SpaceStationService(context, redis.Object).GetById(3);

        Assert.True(result.IsSuccess);
        Assert.Equal("ISS", result.Value!.Name);
    }
}
