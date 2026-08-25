using Moq;
using Orbital.Api.Infrastructure;
using Orbital.Api.Models.External;
using Orbital.Api.Services;

namespace Orbital.Tests.Services;

public class IssServiceTests
{
    [Fact]
    public async Task GetLatestPosition_ReturnsCachedPosition()
    {
        var expected = new IssPositionUpdate
        {
            Latitude = 10,
            Longitude = 20,
            Timestamp = DateTimeOffset.UtcNow
        };
        var redis = new Mock<IRedisService>();
        redis.Setup(item => item.GetAsync<IssPositionUpdate>(CacheKeys.IssPosition))
            .ReturnsAsync(expected);

        var result = await new IssService(redis.Object).GetLatestPosition();

        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public async Task GetLatestPosition_FailsWhenCacheIsEmpty()
    {
        var redis = new Mock<IRedisService>();
        redis.Setup(item => item.GetAsync<IssPositionUpdate>(CacheKeys.IssPosition))
            .ReturnsAsync((IssPositionUpdate?)null);

        var result = await new IssService(redis.Object).GetLatestPosition();

        Assert.False(result.IsSuccess);
        Assert.Equal("ISS position is not available.", result.Error);
    }
}
