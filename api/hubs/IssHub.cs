using Microsoft.AspNetCore.SignalR;
using Orbital.Api.Infrastructure;
using Orbital.Api.Models.External;

namespace Orbital.Api.Hubs;

public class IssHub(IRedisService redis) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var cached = await redis.GetAsync<IssPositionUpdate>(CacheKeys.IssPosition);
        if (cached is not null)
        {
            await Clients.Caller.SendAsync("ReceiveIssPosition", cached);
        }

        await base.OnConnectedAsync();
    }
}