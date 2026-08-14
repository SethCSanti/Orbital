using Orbital.Api.Infrastructure;
using Orbital.Api.Models.External;
using Orbital.Api.Results;

namespace Orbital.Api.Services;

public interface IIssService
{
    Task<Result<IssPositionUpdate>> GetLatestPosition();
}

public class IssService(IRedisService redis) : IIssService
{
    public async Task<Result<IssPositionUpdate>> GetLatestPosition()
    {
        var positionResponse = await redis.GetAsync<IssPositionUpdate>(CacheKeys.IssPosition);

        if (positionResponse is null)
        {
            return Result<IssPositionUpdate>.Failure("ISS position is not available.");
        }

        return Result<IssPositionUpdate>.Success(positionResponse);
    }
}