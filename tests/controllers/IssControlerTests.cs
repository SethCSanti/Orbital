using Moq;
using Orbital.Api.Controllers;
using Orbital.Api.Models.External;
using Orbital.Api.Results;
using Orbital.Api.Services;

namespace Orbital.Tests.Controllers;

public class IssControlerTests
{
    [Fact]
    public async Task GetLatestPosition_DelegatesToService()
    {
        var expected = Result<IssPositionUpdate>.Success(new IssPositionUpdate
        {
            Latitude = 12.5,
            Longitude = -45.25,
            Timestamp = DateTimeOffset.UtcNow
        });
        var service = new Mock<IIssService>();
        service.Setup(item => item.GetLatestPosition()).ReturnsAsync(expected);

        var result = await new IssController(service.Object).GetLatestPosition();

        Assert.Same(expected, result);
        service.Verify(item => item.GetLatestPosition(), Times.Once);
    }
}
