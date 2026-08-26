using Microsoft.EntityFrameworkCore;
using Orbital.Api.Data;
using Orbital.Api.Infrastructure;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Models.Entities;
using Orbital.Api.Results;

namespace Orbital.Api.Services;

public interface ISpaceStationService
{
    /// <summary>Gets all space stations.</summary>
    Task<Result<IEnumerable<SpaceStationDto>>> GetAll();

    /// <summary>Gets a space station by database identifier.</summary>
    Task<Result<SpaceStationDto>> GetById(int id);
}

public class SpaceStationService(OrbitalDbContext context, IRedisService redis)
    : BaseService(context), ISpaceStationService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromDays(7);

    public async Task<Result<IEnumerable<SpaceStationDto>>> GetAll()
    {
        var stations = await GetSpaceStations();
        return Result<IEnumerable<SpaceStationDto>>.Success(stations.Select(entity => new SpaceStationDto(entity)).ToList());
    }

    public async Task<Result<SpaceStationDto>> GetById(int id)
    {
        var stations = await GetSpaceStations();
        var station = stations.FirstOrDefault(entity => entity.Id == id);

        return station is null
            ? Result<SpaceStationDto>.Failure($"Space station with ID {id} was not found.")
            : Result<SpaceStationDto>.Success(new SpaceStationDto(station));
    }

    private async Task<List<SpaceStation>> GetSpaceStations()
    {
        var cached = await redis.GetAsync<List<SpaceStation>>(CacheKeys.SpaceStationData);
        if (cached is { Count: > 0 })
        {
            return cached;
        }

        var stations = await _context.SpaceStations
            .AsNoTracking()
            .OrderBy(entity => entity.Name)
            .ToListAsync();

        if (stations.Count > 0)
        {
            await redis.SetAsync(CacheKeys.SpaceStationData, stations, CacheTtl);
        }
        return stations;
    }
}
