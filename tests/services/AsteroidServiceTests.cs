using Moq;
using Orbital.Api.Infrastructure;
using Orbital.Api.Models.Entities;
using Orbital.Api.Services;

namespace Orbital.Tests.Services;

public class AsteroidServiceTests
{
    [Fact]
    public async Task GetFeed_OrdersDatabaseEntriesByApproachDate()
    {
        await using var context = TestDbContextFactory.Create();
        context.Asteroids.AddRange(
            new Asteroid { NeoReferenceId = "late", Name = "Late", CloseApproachDate = new DateOnly(2026, 8, 28) },
            new Asteroid { NeoReferenceId = "early", Name = "Early", CloseApproachDate = new DateOnly(2026, 8, 25) });
        await context.SaveChangesAsync();

        var redis = new Mock<IRedisService>();
        redis.Setup(item => item.GetAsync<List<Asteroid>>(CacheKeys.AsteroidFeed))
            .ReturnsAsync((List<Asteroid>?)null);
        redis.Setup(item => item.SetAsync(CacheKeys.AsteroidFeed, It.IsAny<List<Asteroid>>(), It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);

        var result = await new AsteroidService(context, redis.Object).GetFeed();

        Assert.True(result.IsSuccess);
        Assert.Equal(["Early", "Late"], result.Value!.Select(item => item.Name));
    }
}
