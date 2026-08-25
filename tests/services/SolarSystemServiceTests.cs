using Orbital.Api.Services;

namespace Orbital.Tests.Services;

public class SolarSystemServiceTests
{
    [Fact]
    public async Task GetPositions_ReturnsAllEightPlanetsAtRequestedTime()
    {
        await using var context = TestDbContextFactory.Create();
        var service = new SolarSystemService(context);

        var result = await service.GetPositions(new DateTimeOffset(2000, 1, 1, 12, 0, 0, TimeSpan.Zero));

        Assert.True(result.IsSuccess);
        Assert.Equal(8, result.Value!.Count());
        Assert.Equal(
            ["Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune"],
            result.Value!.Select(item => item.Name));
        Assert.All(result.Value!, item =>
        {
            Assert.True(double.IsFinite(item.X));
            Assert.True(double.IsFinite(item.Y));
            Assert.True(double.IsFinite(item.Z));
            Assert.True(item.OrbitalPeriodDays > 0);
        });
    }

    [Fact]
    public async Task GetPositions_IsDeterministicForSameTimestamp()
    {
        await using var context = TestDbContextFactory.Create();
        var service = new SolarSystemService(context);
        var at = new DateTimeOffset(2026, 8, 24, 12, 0, 0, TimeSpan.Zero);

        var first = await service.GetPositions(at);
        var second = await service.GetPositions(at);

        Assert.Equal(first.Value, second.Value);
    }
}
