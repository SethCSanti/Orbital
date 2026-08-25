using Moq;
using Orbital.Api.Infrastructure;
using Orbital.Api.Models.Entities;
using Orbital.Api.Services;

namespace Orbital.Tests.Services;

public class ApodServiceTests
{
    [Fact]
    public async Task GetLatest_ReturnsNewestDatabaseEntryAndCachesIt()
    {
        await using var context = TestDbContextFactory.Create();
        context.ApodEntries.AddRange(
            new ApodEntry { Date = new DateOnly(2026, 8, 22), Title = "Older" },
            new ApodEntry { Date = new DateOnly(2026, 8, 24), Title = "Newest" });
        await context.SaveChangesAsync();

        var redis = new Mock<IRedisService>();
        redis.Setup(item => item.GetAsync<ApodEntry>(CacheKeys.Apod))
            .ReturnsAsync((ApodEntry?)null);
        redis.Setup(item => item.SetAsync(CacheKeys.Apod, It.IsAny<ApodEntry>(), It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);

        var result = await new ApodService(context, redis.Object).GetLatest();

        Assert.True(result.IsSuccess);
        Assert.Equal("Newest", result.Value!.Title);
        redis.Verify(item => item.SetAsync(CacheKeys.Apod, It.Is<ApodEntry>(entry => entry.Title == "Newest"), It.IsAny<TimeSpan>()), Times.Once);
    }
}
