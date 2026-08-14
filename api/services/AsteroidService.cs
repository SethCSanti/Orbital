using Microsoft.EntityFrameworkCore;
using Orbital.Api.Data;
using Orbital.Api.Infrastructure;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Models.Entities;
using Orbital.Api.Results;

namespace Orbital.Api.Services;

public interface IAsteroidService
{
    /// <summary>Gets the current near-Earth asteroid feed.</summary>
    Task<Result<IEnumerable<AsteroidDto>>> GetFeed();
}

public class AsteroidService(OrbitalDbContext context, IRedisService redis)
    : BaseService(context), IAsteroidService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(24);

    public async Task<Result<IEnumerable<AsteroidDto>>> GetFeed()
    {
        var asteroids = await redis.GetAsync<List<Asteroid>>(CacheKeys.AsteroidFeed);
        if (asteroids is null)
        {
            asteroids = await _context.Asteroids
                .AsNoTracking()
                .OrderBy(entity => entity.CloseApproachDate)
                .ToListAsync();

            await redis.SetAsync(CacheKeys.AsteroidFeed, asteroids, CacheTtl);
        }

        return Result<IEnumerable<AsteroidDto>>.Success(asteroids.Select(entity => new AsteroidDto(entity)).ToList());
    }
}
