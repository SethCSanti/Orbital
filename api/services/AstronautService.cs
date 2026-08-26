using Microsoft.EntityFrameworkCore;
using Orbital.Api.Data;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Results;

namespace Orbital.Api.Services;

public interface IAstronautService
{
    Task<Result<PagedResult<AstronautDto>>> GetPage(int page, int pageSize, string? search);
    Task<Result<AstronautDetailDto>> GetById(int id);
}

public class AstronautService(OrbitalDbContext context) : BaseService(context), IAstronautService
{
    public async Task<Result<PagedResult<AstronautDto>>> GetPage(int page, int pageSize, string? search)
    {
        var query = _context.Astronauts.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(astronaut => astronaut.Name.Contains(term) ||
                                             (astronaut.Nationality != null && astronaut.Nationality.Contains(term)));
        }

        var total = await query.CountAsync();
        var entities = await query.OrderBy(astronaut => astronaut.Name)
            .ThenBy(astronaut => astronaut.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var items = entities.Select(astronaut => new AstronautDto(astronaut)).ToList();

        return Result<PagedResult<AstronautDto>>.Success(new PagedResult<AstronautDto>(items, page, pageSize, total));
    }

    public async Task<Result<AstronautDetailDto>> GetById(int id)
    {
        var astronaut = await _context.Astronauts.AsNoTracking().FirstOrDefaultAsync(entity => entity.Id == id);
        if (astronaut is null)
        {
            return Result<AstronautDetailDto>.Failure($"Astronaut with ID {id} was not found.");
        }

        var launchEntities = await _context.Launches.AsNoTracking()
            .Include(launch => launch.Rocket)
            .Include(launch => launch.Mission)
            .Include(launch => launch.Crew)
            .Where(launch => launch.Crew.Any(crew => crew.Id == id))
            .OrderByDescending(launch => launch.Net)
            .Take(200)
            .ToListAsync();
        var launches = launchEntities.Select(launch => new RelatedLaunchDto(launch)).ToList();

        return Result<AstronautDetailDto>.Success(new AstronautDetailDto(new AstronautDto(astronaut), launches));
    }
}
