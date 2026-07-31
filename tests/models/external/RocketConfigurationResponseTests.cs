using System.Text.Json;
using Orbital.Api.Models.External;

namespace Orbital.Tests.Models.External;

public class RocketConfigurationResponseTests
{
    [Theory]
    [InlineData("50000000", 50000000)]
    [InlineData("\"50000000\"", 50000000)]
    public void LaunchCost_DeserializesNumericValues(string jsonValue, decimal expected)
    {
        var response = Deserialize(jsonValue);

        Assert.Equal(expected, response.LaunchCost);
    }

    [Theory]
    [InlineData("\"Unknown\"")]
    [InlineData("null")]
    public void LaunchCost_DeserializesUnsupportedValuesAsNull(string jsonValue)
    {
        var response = Deserialize(jsonValue);

        Assert.Null(response.LaunchCost);
    }

    [Fact]
    public void LaunchCost_SerializesAsNumber()
    {
        var response = new RocketConfigurationResponse { LaunchCost = 50000000m };

        var json = JsonSerializer.Serialize(response);

        Assert.Contains("\"launch_cost\":50000000", json);
    }

    [Fact]
    public void LaunchCost_SerializesNull()
    {
        var response = new RocketConfigurationResponse { LaunchCost = null };

        var json = JsonSerializer.Serialize(response);

        Assert.Contains("\"launch_cost\":null", json);
    }

    private static RocketConfigurationResponse Deserialize(string launchCost)
    {
        var json = $$"""{"launch_cost":{{launchCost}}}""";
        return JsonSerializer.Deserialize<RocketConfigurationResponse>(json)!;
    }
}
