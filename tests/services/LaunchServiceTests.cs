using Moq;
using Orbital.Api.Infrastructure;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Models.Entities;
using Orbital.Api.Services;

namespace Orbital.Tests.Services;

public class LaunchServiceTests
{
    [Fact]
    public async Task GetUpcoming_FiltersByRocketAndOrdersByNet()
    {
        await using var context = TestDbContextFactory.Create();
        var now = DateTimeOffset.UtcNow;
        context.Launches.AddRange(
            CreateLaunch("Later Falcon", "Falcon 9", now.AddDays(3)),
            CreateLaunch("Soon Falcon", "Falcon 9", now.AddDays(1)),
            CreateLaunch("Soon Atlas", "Atlas V", now.AddDays(2)),
            CreateLaunch("Past Falcon", "Falcon 9", now.AddDays(-1)));
        await context.SaveChangesAsync();

        var redis = new Mock<IRedisService>();
        redis.Setup(item => item.GetAsync<List<LaunchDto>>(CacheKeys.UpcomingLaunches))
            .ReturnsAsync((List<LaunchDto>?)null);
        redis.Setup(item => item.SetAsync(CacheKeys.UpcomingLaunches, It.IsAny<List<LaunchDto>>(), It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);

        var result = await new LaunchService(context, redis.Object).GetUpcoming("falcon 9");

        Assert.True(result.IsSuccess);
        Assert.Equal(["Soon Falcon", "Later Falcon"], result.Value!.Select(item => item.Name));
    }

    private static Launch CreateLaunch(string name, string rocketName, DateTimeOffset net) => new()
    {
        Name = name,
        StatusName = "Go",
        Net = net,
        WindowStart = net,
        WindowEnd = net.AddMinutes(10),
        Rocket = new Rocket { Name = rocketName },
        Mission = new Mission { Name = $"{name} mission", OrbitAbbrev = "LEO" }
    };
}
