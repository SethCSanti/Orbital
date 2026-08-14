using Microsoft.EntityFrameworkCore;
using Orbital.Api.Data;
using Orbital.Api.Infrastructure;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Models.Entities;
using Orbital.Api.Results;

namespace Orbital.Api.Services;

public interface IRocketService
{
    /// <summary>Gets all rockets.</summary>
    Task<Result<IEnumerable<RocketDto>>> GetAll();

    /// <summary>Gets a rocket by name.</summary>
    Task<Result<RocketDto>> GetByName(string name);

    /// <summary>Gets the rockets whose names are requested for comparison.</summary>
    Task<Result<IEnumerable<RocketDto>>> Compare(List<string> names);
}

public class RocketService(OrbitalDbContext context, IRedisService redis)
    : BaseService(context), IRocketService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(24);

    public async Task<Result<IEnumerable<RocketDto>>> GetAll()
    {
        var rockets = await GetRockets();
        return Result<IEnumerable<RocketDto>>.Success(rockets.Select(entity => new RocketDto(entity)).ToList());
    }

    public async Task<Result<RocketDto>> GetByName(string name)
    {
        var rockets = await GetRockets();
        var rocket = rockets.FirstOrDefault(entity =>
            string.Equals(entity.Name, name, StringComparison.OrdinalIgnoreCase));

        return rocket is null
            ? Result<RocketDto>.Failure($"Rocket '{name}' was not found.")
            : Result<RocketDto>.Success(new RocketDto(rocket));
    }

    public async Task<Result<IEnumerable<RocketDto>>> Compare(List<string> names)
    {
        var rockets = await GetRockets();
        var requestedNames = names.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var matches = rockets
            .Where(entity => requestedNames.Contains(entity.Name))
            .Select(entity => new RocketDto(entity))
            .ToList();

        return Result<IEnumerable<RocketDto>>.Success(matches);
    }

    private async Task<List<Rocket>> GetRockets()
    {
        var cached = await redis.GetAsync<List<Rocket>>(CacheKeys.RocketData);
        if (cached is not null)
        {
            return cached;
        }

        var rockets = await _context.Rockets
            .AsNoTracking()
            .OrderBy(entity => entity.Name)
            .ThenBy(entity => entity.Variant)
            .ToListAsync();

        await redis.SetAsync(CacheKeys.RocketData, rockets, CacheTtl);
        return rockets;
    }
}
