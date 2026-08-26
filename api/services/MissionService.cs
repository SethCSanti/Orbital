using Microsoft.EntityFrameworkCore;
using Orbital.Api.Data;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Results;

namespace Orbital.Api.Services;

public interface IMissionService
{
    Task<Result<PagedResult<MissionDto>>> GetPage(int page, int pageSize, string? search, string? type, string? orbitAbbrev);
    Task<Result<MissionDetailDto>> GetById(int id);
}

public class MissionService(OrbitalDbContext context) : BaseService(context), IMissionService
{
    public async Task<Result<PagedResult<MissionDto>>> GetPage(int page, int pageSize, string? search, string? type, string? orbitAbbrev)
    {
        var query = _context.Missions.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(mission => mission.Name.Contains(term) || mission.Description.Contains(term));
        }
        if (!string.IsNullOrWhiteSpace(type)) query = query.Where(mission => mission.Type == type);
        if (!string.IsNullOrWhiteSpace(orbitAbbrev)) query = query.Where(mission => mission.OrbitAbbrev == orbitAbbrev);

        var total = await query.CountAsync();
        var entities = await query.OrderByDescending(mission => mission.LastLaunchDate)
            .ThenByDescending(mission => mission.LaunchDesignator)
            .ThenBy(mission => mission.Name)
            .ThenBy(mission => mission.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var items = entities.Select(mission => new MissionDto(mission)).ToList();

        var filterMetadata = new Dictionary<string, IReadOnlyList<string>>
        {
            ["types"] = await _context.Missions.AsNoTracking().Where(mission => mission.Type != "").Select(mission => mission.Type).Distinct().OrderBy(value => value).ToListAsync(),
            ["orbits"] = await _context.Missions.AsNoTracking().Where(mission => mission.OrbitAbbrev != "").Select(mission => mission.OrbitAbbrev).Distinct().OrderBy(value => value).ToListAsync()
        };

        return Result<PagedResult<MissionDto>>.Success(new PagedResult<MissionDto>(items, page, pageSize, total, filterMetadata));
    }

    public async Task<Result<MissionDetailDto>> GetById(int id)
    {
        var mission = await _context.Missions.AsNoTracking().FirstOrDefaultAsync(entity => entity.Id == id);
        if (mission is null)
        {
            return Result<MissionDetailDto>.Failure($"Mission with ID {id} was not found.");
        }

        var launchEntities = await _context.Launches.AsNoTracking()
            .Include(launch => launch.Rocket)
            .Include(launch => launch.Mission)
            .Where(launch => launch.MissionId == id)
            .OrderByDescending(launch => launch.Net)
            .Take(200)
            .ToListAsync();
        var launches = launchEntities.Select(launch => new RelatedLaunchDto(launch)).ToList();

        return Result<MissionDetailDto>.Success(new MissionDetailDto(new MissionDto(mission), launches));
    }
}
