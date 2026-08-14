using Microsoft.EntityFrameworkCore;
using Orbital.Api.Data;
using Orbital.Api.Infrastructure;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Models.Entities;
using Orbital.Api.Results;

namespace Orbital.Api.Services;

public interface IApodService
{
    /// <summary>Gets the latest astronomy picture of the day entry.</summary>
    Task<Result<ApodEntryDto>> GetLatest();

    /// <summary>Gets the astronomy picture of the day entry for a date.</summary>
    Task<Result<ApodEntryDto>> GetByDate(DateOnly date);
}

public class ApodService(OrbitalDbContext context, IRedisService redis)
    : BaseService(context), IApodService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(24);

    public async Task<Result<ApodEntryDto>> GetLatest()
    {
        var cached = await redis.GetAsync<ApodEntry>(CacheKeys.Apod);
        if (cached is not null)
        {
            return Result<ApodEntryDto>.Success(new ApodEntryDto(cached));
        }

        var entry = await _context.ApodEntries
            .AsNoTracking()
            .OrderByDescending(entity => entity.Date)
            .FirstOrDefaultAsync();

        if (entry is null)
        {
            return Result<ApodEntryDto>.Failure("No APOD entries were found.");
        }

        await redis.SetAsync(CacheKeys.Apod, entry, CacheTtl);
        return Result<ApodEntryDto>.Success(new ApodEntryDto(entry));
    }

    public async Task<Result<ApodEntryDto>> GetByDate(DateOnly date)
    {
        var cached = await redis.GetAsync<ApodEntry>(CacheKeys.Apod);
        if (cached?.Date == date)
        {
            return Result<ApodEntryDto>.Success(new ApodEntryDto(cached));
        }

        var entry = await _context.ApodEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.Date == date);

        if (entry is null)
        {
            return Result<ApodEntryDto>.Failure($"No APOD entry was found for {date:yyyy-MM-dd}.");
        }

        await redis.SetAsync(CacheKeys.Apod, entry, CacheTtl);
        return Result<ApodEntryDto>.Success(new ApodEntryDto(entry));
    }
}
