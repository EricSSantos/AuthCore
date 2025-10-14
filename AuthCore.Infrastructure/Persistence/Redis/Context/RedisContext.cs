using AuthCore.Domain.Commons.Interfaces.Persistence;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace AuthCore.Infrastructure.Persistence.Redis.Context
{
    public sealed class RedisContext : IRedisContext
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IDatabase _database;

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public RedisContext(
            IDistributedCache distributedCache,
            IConnectionMultiplexer connection)
        {
            _distributedCache = distributedCache;
            _database = connection.GetDatabase();
        }

        public async Task WriteObject<T>(string key, T value, TimeSpan ttl)
        {
            var serializedValue = JsonSerializer.Serialize(value, JsonOptions);

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            };

            await _distributedCache.SetStringAsync(key, serializedValue, cacheOptions);
        }

        public async Task<T?> ReadObject<T>(string key)
        {
            var serializedValue = await _distributedCache.GetStringAsync(key);

            if (string.IsNullOrWhiteSpace(serializedValue))
                return default;

            return JsonSerializer.Deserialize<T>(serializedValue, JsonOptions);
        }

        public async Task DeleteKey(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }

        #region Index Operations

        public async Task AddIndex(string key, string value, TimeSpan ttl)
        {
            await _database.SetAddAsync(key, value);
            await _database.KeyExpireAsync(key, ttl);
        }

        public async Task<IEnumerable<string>> GetIndexMembers(string key)
        {
            var rawMembers = await _database.SetMembersAsync(key);
            var indexValues = new List<string>();

            foreach (var rawValue in rawMembers)
                indexValues.Add(rawValue.ToString());

            return indexValues;
        }

        public async Task RemoveIndex(string key, string value)
        {
            await _database.SetRemoveAsync(key, value);
        }

        #endregion
    }
}
