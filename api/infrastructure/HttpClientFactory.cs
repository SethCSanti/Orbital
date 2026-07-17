using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
namespace Orbital.Api.Infrastructure;

public static class HttpClientFactory
{
    public static IServiceCollection AddOrbitalHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        var nasaApiKey = configuration["Nasa:ApiKey"];
        
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
        });

        services.AddHttpClient("ExoplanetArchive", client =>
        {
            client.BaseAddress = new Uri("https://exoplanetarchive.ipac.caltech.edu/");
        });

        return services;
    }
}