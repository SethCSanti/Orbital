using Microsoft.EntityFrameworkCore;
using Orbital.Api.Data;
using Orbital.Api.Models.Entities;
using Orbital.Api.Results;

namespace Orbital.Api.Services;

public record CatalogStatusDto(
    string Catalog,
    string Status,
    int CurrentPage,
    int PageSize,
    int? TotalAvailable,
    int RecordsImported,
    DateTimeOffset? LastStartedAt,
    DateTimeOffset? LastCompletedAt,
    DateTimeOffset UpdatedAt,
    string? LastError);

public interface ICatalogStatusService
{
    Task<Result<IReadOnlyList<CatalogStatusDto>>> GetAll();
}

public class CatalogStatusService(OrbitalDbContext context) : ICatalogStatusService
{
    public async Task<Result<IReadOnlyList<CatalogStatusDto>>> GetAll()
    {
        var states = await context.CatalogSyncStates.AsNoTracking().OrderBy(state => state.Catalog).ToListAsync();
        if (states.All(state => state.Catalog != "launch-history")) states.Add(Pending("launch-history"));
        return Result<IReadOnlyList<CatalogStatusDto>>.Success(states.OrderBy(state => state.Catalog).Select(ToDto).ToList());
    }

    public static CatalogSyncState Pending(string catalog) => new() { Catalog = catalog };

    private static CatalogStatusDto ToDto(CatalogSyncState state) => new(
        state.Catalog,
        state.Status,
        state.CurrentPage,
        state.PageSize,
        state.TotalAvailable,
        state.RecordsImported,
        state.LastStartedAt,
        state.LastCompletedAt,
        state.UpdatedAt,
        state.LastError);
}
