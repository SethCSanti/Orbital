using Moq;
using Orbital.Api.Controllers;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Results;
using Orbital.Api.Services;

namespace Orbital.Tests.Controllers;

public class SolarSystemControllerTests
{
    [Fact]
    public async Task GetPositions_ForwardsRequestedTimestamp()
    {
        var at = new DateTimeOffset(2026, 8, 24, 12, 0, 0, TimeSpan.Zero);
        var expected = Result<IEnumerable<PlanetPositionDto>>.Success(
            [new PlanetPositionDto("Earth", 1, 0, 0, 365.256)]);
        var service = new Mock<ISolarSystemService>();
        service.Setup(item => item.GetPositions(at)).ReturnsAsync(expected);

        var result = await new SolarSystemController(service.Object).GetPositions(at);

        Assert.Same(expected, result);
        service.Verify(item => item.GetPositions(at), Times.Once);
    }
}
