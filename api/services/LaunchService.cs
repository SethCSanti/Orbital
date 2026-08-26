using Microsoft.EntityFrameworkCore;
using Orbital.Api.Data;
using Orbital.Api.Infrastructure;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Results;

namespace Orbital.Api.Services;

public interface ILaunchService
{
    /// <summary>Gets upcoming launches, optionally filtered by rocket name.</summary>
    Task<Result<IEnumerable<LaunchDto>>> GetUpcoming(string? rocketName = null);

    /// <summary>Gets past launches, optionally filtered by rocket name.</summary>
    Task<Result<IEnumerable<LaunchDto>>> GetPast(string? rocketName = null);
}

public class LaunchService(OrbitalDbContext context, IRedisService redis)
    : BaseService(context), ILaunchService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(20);

    public async Task<Result<IEnumerable<LaunchDto>>> GetUpcoming(string? rocketName = null)
    {
        var launches = await GetLaunches(CacheKeys.UpcomingLaunches, upcoming: true);
        var now = DateTimeOffset.UtcNow;
        var filtered = launches.Where(launch => launch.Net >= now);

        if (!string.IsNullOrWhiteSpace(rocketName))
        {
            filtered = filtered.Where(launch =>
                string.Equals(launch.RocketName, rocketName, StringComparison.OrdinalIgnoreCase));
        }

        return Result<IEnumerable<LaunchDto>>.Success(filtered.OrderBy(launch => launch.Net).ToList());
    }

    public async Task<Result<IEnumerable<LaunchDto>>> GetPast(string? rocketName = null)
    {
        var launches = await GetLaunches(CacheKeys.PastLaunches, upcoming: false);
        var now = DateTimeOffset.UtcNow;
        var filtered = launches.Where(launch => launch.Net < now);

        if (!string.IsNullOrWhiteSpace(rocketName))
        {
            filtered = filtered.Where(launch =>
                string.Equals(launch.RocketName, rocketName, StringComparison.OrdinalIgnoreCase));
        }

        return Result<IEnumerable<LaunchDto>>.Success(filtered.OrderByDescending(launch => launch.Net).ToList());
    }

    private async Task<List<LaunchDto>> GetLaunches(string cacheKey, bool upcoming)
    {
        var cached = await redis.GetAsync<List<LaunchDto>>(cacheKey);
        if (cached is not null)
        {
            return cached;
        }

        var now = DateTimeOffset.UtcNow;
        var query = _context.Launches
            .AsNoTracking()
            .Include(entity => entity.Rocket)
            .Include(entity => entity.Mission)
            .AsQueryable();

        query = upcoming
            ? query.Where(entity => entity.Net >= now).OrderBy(entity => entity.Net)
            : query.Where(entity => entity.Net < now).OrderByDescending(entity => entity.Net);

        var launches = await query.ToListAsync();
        var dtos = launches.Select(entity => new LaunchDto(entity)).ToList();
        await redis.SetAsync(cacheKey, dtos, CacheTtl);
        return dtos;
    }
}
