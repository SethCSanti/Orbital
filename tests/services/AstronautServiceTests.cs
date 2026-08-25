using Moq;
using Orbital.Api.Infrastructure;
using Orbital.Api.Models.Entities;
using Orbital.Api.Services;

namespace Orbital.Tests.Services;

public class AstronautServiceTests
{
    [Fact]
    public async Task GetById_ReturnsMatchingCachedAstronaut()
    {
        await using var context = TestDbContextFactory.Create();
        var redis = new Mock<IRedisService>();
        redis.Setup(item => item.GetAsync<List<Astronaut>>(CacheKeys.Astronauts))
            .ReturnsAsync([
                new Astronaut { Id = 7, Name = "Ada Astronaut" },
                new Astronaut { Id = 8, Name = "Bert Astronaut" }
            ]);

        var result = await new AstronautService(context, redis.Object).GetById(8);

        Assert.True(result.IsSuccess);
        Assert.Equal("Bert Astronaut", result.Value!.Name);
    }

    [Fact]
    public async Task GetById_ReturnsFailureForUnknownId()
    {
        await using var context = TestDbContextFactory.Create();
        var redis = new Mock<IRedisService>();
        redis.Setup(item => item.GetAsync<List<Astronaut>>(CacheKeys.Astronauts))
            .ReturnsAsync(new List<Astronaut>());

        var result = await new AstronautService(context, redis.Object).GetById(99);

        Assert.False(result.IsSuccess);
        Assert.Contains("99", result.Error);
    }
}
