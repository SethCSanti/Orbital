using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orbital.Api.Infrastructure;

namespace Orbital.Tests.Infrastructure;

public class HttpClientFactoryTests
{
    [Fact]
    public void SpaceDevsClient_UsesConfiguredToken()
    {
        using var provider = BuildProvider("test-token");

        var client = provider.GetRequiredService<IHttpClientFactory>().CreateClient("SpaceDevs");

        Assert.Equal("https://ll.thespacedevs.com/2.2.0/", client.BaseAddress!.ToString());
        Assert.Equal("Token test-token", client.DefaultRequestHeaders.Authorization!.ToString());
    }

    [Fact]
    public void SpaceDevsClient_AllowsAnonymousRequestsWithoutToken()
    {
        using var provider = BuildProvider(null);

        var client = provider.GetRequiredService<IHttpClientFactory>().CreateClient("SpaceDevs");

        Assert.Null(client.DefaultRequestHeaders.Authorization);
    }

    private static ServiceProvider BuildProvider(string? apiKey)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SpaceDevs:ApiKey"] = apiKey
            })
            .Build();
        var services = new ServiceCollection();
        services.AddOrbitalHttpClients(configuration);
        return services.BuildServiceProvider();
    }
}
