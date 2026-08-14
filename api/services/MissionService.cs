using Microsoft.EntityFrameworkCore;
using Orbital.Api.Data;
using Orbital.Api.Infrastructure;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Models.Entities;
using Orbital.Api.Results;

namespace Orbital.Api.Services;

public interface IMissionService
{
    /// <summary>Gets missions, optionally filtered by type and orbit abbreviation.</summary>
    Task<Result<IEnumerable<MissionDto>>> GetAll(string? type = null, string? orbitAbbrev = null);
}

public class MissionService(OrbitalDbContext context, IRedisService redis)
    : BaseService(context), IMissionService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(24);

    public async Task<Result<IEnumerable<MissionDto>>> GetAll(string? type = null, string? orbitAbbrev = null)
    {
        var missions = await redis.GetAsync<List<Mission>>(CacheKeys.MissionHistory);
        if (missions is null)
        {
            missions = await _context.Missions
                .AsNoTracking()
                .OrderBy(entity => entity.Name)
                .ToListAsync();

            await redis.SetAsync(CacheKeys.MissionHistory, missions, CacheTtl);
        }

        IEnumerable<Mission> filtered = missions;
        if (!string.IsNullOrWhiteSpace(type))
        {
            filtered = filtered.Where(entity => string.Equals(entity.Type, type, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(orbitAbbrev))
        {
            filtered = filtered.Where(entity =>
                string.Equals(entity.OrbitAbbrev, orbitAbbrev, StringComparison.OrdinalIgnoreCase));
        }

        return Result<IEnumerable<MissionDto>>.Success(filtered.Select(entity => new MissionDto(entity)).ToList());
    }
}
