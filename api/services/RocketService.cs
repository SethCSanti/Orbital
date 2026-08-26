using Microsoft.EntityFrameworkCore;
using Orbital.Api.Data;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Results;

namespace Orbital.Api.Services;

public interface IRocketService
{
    Task<Result<PagedResult<RocketDto>>> GetPage(int page, int pageSize, string? search);
    Task<Result<RocketDetailDto>> GetById(int id);
    Task<Result<RocketDto>> GetByName(string name);
    Task<Result<IEnumerable<RocketDto>>> Compare(List<string> names);
}

public class RocketService(OrbitalDbContext context) : BaseService(context), IRocketService
{
    public async Task<Result<PagedResult<RocketDto>>> GetPage(int page, int pageSize, string? search)
    {
        var query = _context.Rockets.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(rocket => rocket.Name.Contains(term) || rocket.FullName.Contains(term) || rocket.Family.Contains(term));
        }

        var total = await query.CountAsync();
        var entities = await query.OrderBy(rocket => rocket.Name)
            .ThenBy(rocket => rocket.Variant)
            .ThenBy(rocket => rocket.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var items = entities.Select(rocket => new RocketDto(rocket)).ToList();

        return Result<PagedResult<RocketDto>>.Success(new PagedResult<RocketDto>(items, page, pageSize, total));
    }

    public async Task<Result<RocketDetailDto>> GetById(int id)
    {
        var rocket = await _context.Rockets.AsNoTracking().FirstOrDefaultAsync(entity => entity.Id == id);
        if (rocket is null)
        {
            return Result<RocketDetailDto>.Failure($"Rocket with ID {id} was not found.");
        }

        var launchEntities = await _context.Launches.AsNoTracking()
            .Include(launch => launch.Rocket)
            .Include(launch => launch.Mission)
            .Include(launch => launch.Crew)
            .Where(launch => launch.RocketId == id)
            .OrderByDescending(launch => launch.Net)
            .Take(200)
            .ToListAsync();
        var launches = launchEntities.Select(launch => new RelatedLaunchDto(launch)).ToList();

        return Result<RocketDetailDto>.Success(new RocketDetailDto(new RocketDto(rocket), launches));
    }

    public async Task<Result<RocketDto>> GetByName(string name)
    {
        var rocket = await _context.Rockets.AsNoTracking()
            .Where(entity => entity.Name.ToLower() == name.ToLower())
            .OrderBy(entity => entity.Variant)
            .FirstOrDefaultAsync();

        return rocket is null
            ? Result<RocketDto>.Failure($"Rocket '{name}' was not found.")
            : Result<RocketDto>.Success(new RocketDto(rocket));
    }

    public async Task<Result<IEnumerable<RocketDto>>> Compare(List<string> names)
    {
        var requestedNames = names.Where(name => !string.IsNullOrWhiteSpace(name))
            .Take(4)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var entities = await _context.Rockets.AsNoTracking()
            .Where(rocket => requestedNames.Contains(rocket.Name))
            .OrderBy(rocket => rocket.Name)
            .ToListAsync();
        var rockets = entities.Select(rocket => new RocketDto(rocket)).ToList();

        return Result<IEnumerable<RocketDto>>.Success(rockets);
    }
}
