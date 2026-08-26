using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
namespace Orbital.Api.Infrastructure;

public static class HttpClientFactory
{
    public static IServiceCollection AddOrbitalHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("Nasa", client =>
        {
            client.BaseAddress = new Uri("https://api.nasa.gov/");
        });

        services.AddHttpClient("SpaceX", client =>
        {
            client.BaseAddress = new Uri("https://api.spacexdata.com/v4/");
        });

        services.AddHttpClient("OpenNotify", client =>
        {
            client.BaseAddress = new Uri("http://api.open-notify.org/");
        });

        services.AddHttpClient("Celestrak", client =>
        {
            client.BaseAddress = new Uri("https://celestrak.org/");
        });

        services.AddHttpClient("SpaceDevs", client =>
        {
            client.BaseAddress = new Uri("https://ll.thespacedevs.com/2.2.0/");

            var apiKey = configuration["SpaceDevs:ApiKey"];
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                // LL2 uses DRF token authentication; leaving this unset preserves anonymous access.
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Token", apiKey);
            }
        });

        return services;
    }
}
