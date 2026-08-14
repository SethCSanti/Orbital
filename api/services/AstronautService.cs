using Microsoft.EntityFrameworkCore;
using Orbital.Api.Data;
using Orbital.Api.Infrastructure;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Models.Entities;
using Orbital.Api.Results;

namespace Orbital.Api.Services;

public interface IAstronautService
{
    /// <summary>Gets all astronauts.</summary>
    Task<Result<IEnumerable<AstronautDto>>> GetAll();

    /// <summary>Gets an astronaut by database identifier.</summary>
    Task<Result<AstronautDto>> GetById(int id);
}

public class AstronautService(OrbitalDbContext context, IRedisService redis)
    : BaseService(context), IAstronautService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(24);

    public async Task<Result<IEnumerable<AstronautDto>>> GetAll()
    {
        var astronauts = await GetAstronauts();
        return Result<IEnumerable<AstronautDto>>.Success(astronauts.Select(entity => new AstronautDto(entity)).ToList());
    }

    public async Task<Result<AstronautDto>> GetById(int id)
    {
        var astronauts = await GetAstronauts();
        var astronaut = astronauts.FirstOrDefault(entity => entity.Id == id);

        return astronaut is null
            ? Result<AstronautDto>.Failure($"Astronaut with ID {id} was not found.")
            : Result<AstronautDto>.Success(new AstronautDto(astronaut));
    }

    private async Task<List<Astronaut>> GetAstronauts()
    {
        var cached = await redis.GetAsync<List<Astronaut>>(CacheKeys.Astronauts);
        if (cached is not null)
        {
            return cached;
        }

        var astronauts = await _context.Astronauts
            .AsNoTracking()
            .OrderBy(entity => entity.Name)
            .ToListAsync();

        await redis.SetAsync(CacheKeys.Astronauts, astronauts, CacheTtl);
        return astronauts;
    }
}
