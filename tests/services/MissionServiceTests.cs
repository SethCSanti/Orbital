using Moq;
using Orbital.Api.Infrastructure;
using Orbital.Api.Models.Entities;
using Orbital.Api.Services;

namespace Orbital.Tests.Services;

public class MissionServiceTests
{
    [Fact]
    public async Task GetAll_FiltersByTypeAndOrbit()
    {
        await using var context = TestDbContextFactory.Create();
        var redis = new Mock<IRedisService>();
        redis.Setup(item => item.GetAsync<List<Mission>>(CacheKeys.MissionHistory))
            .ReturnsAsync([
                new Mission { Name = "Apollo", Type = "Human", OrbitAbbrev = "LEO" },
                new Mission { Name = "Voyager", Type = "Robotic", OrbitAbbrev = "ESC" },
                new Mission { Name = "Gemini", Type = "Human", OrbitAbbrev = "LEO" }
            ]);

        var result = await new MissionService(context, redis.Object).GetAll("human", "leo");

        Assert.True(result.IsSuccess);
        Assert.Equal(["Apollo", "Gemini"], result.Value!.Select(item => item.Name));
    }
}
