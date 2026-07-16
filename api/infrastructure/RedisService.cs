using StackExchange.Redis;
using System.Text.Json;
namespace Orbital.Api.Infrastructure;

public interface IRedisService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan ttl);
    Task DeleteAsync(string key);
}

public class RedisService : IRedisService
{
    private readonly IConnectionMultiplexer _connection;

    public RedisService(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var db = _connection.GetDatabase();
        var value = await db.StringGetAsync(key);
        if (value.IsNullOrEmpty)
        {
            return default;
        }
        return JsonSerializer.Deserialize<T>((string)value!);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl)
    {
        var db = _connection.GetDatabase();
        var jsonString = JsonSerializer.Serialize(value);
        await db.StringSetAsync(key, jsonString, ttl);
    }

    public async Task DeleteAsync(string key)
    {
        var db = _connection.GetDatabase();
        await db.KeyDeleteAsync(key);
    }
}