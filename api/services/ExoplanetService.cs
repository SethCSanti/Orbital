using Microsoft.EntityFrameworkCore;
using Orbital.Api.Data;
using Orbital.Api.Infrastructure;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Models.Entities;
using Orbital.Api.Results;

namespace Orbital.Api.Services;

public interface IExoplanetService
{
    /// <summary>Gets exoplanets, optionally filtered by discovery method and discovery-year range.</summary>
    Task<Result<IEnumerable<ExoplanetDto>>> GetAll(
        string? discoveryMethod = null,
        int? minYear = null,
        int? maxYear = null);
}

public class ExoplanetService(OrbitalDbContext context, IRedisService redis)
    : BaseService(context), IExoplanetService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(24);

    public async Task<Result<IEnumerable<ExoplanetDto>>> GetAll(
        string? discoveryMethod = null,
        int? minYear = null,
        int? maxYear = null)
    {
        var exoplanets = await redis.GetAsync<List<Exoplanet>>(CacheKeys.ExoplanetData);
        if (exoplanets is null)
        {
            exoplanets = await _context.Exoplanets
                .AsNoTracking()
                .OrderBy(entity => entity.PlanetName)
                .ToListAsync();

            await redis.SetAsync(CacheKeys.ExoplanetData, exoplanets, CacheTtl);
        }

        IEnumerable<Exoplanet> filtered = exoplanets;
        if (!string.IsNullOrWhiteSpace(discoveryMethod))
        {
            filtered = filtered.Where(entity =>
                string.Equals(entity.DiscoveryMethod, discoveryMethod, StringComparison.OrdinalIgnoreCase));
        }

        if (minYear.HasValue)
        {
            filtered = filtered.Where(entity => entity.DiscoveryYear >= minYear.Value);
        }

        if (maxYear.HasValue)
        {
            filtered = filtered.Where(entity => entity.DiscoveryYear <= maxYear.Value);
        }

        return Result<IEnumerable<ExoplanetDto>>.Success(filtered.Select(entity => new ExoplanetDto(entity)).ToList());
    }
}
