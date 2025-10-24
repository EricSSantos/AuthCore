using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Infrastructure.Persistence.Redis.Mappings;
using StackExchange.Redis;
using System.Text.Json;

namespace AuthCore.Infrastructure.Persistence.Redis.Repositories
{
    public sealed class SessionRepository : ISessionRepository
    {
        #region Constants

        private const string PREFIX = "sessions:";
        private static readonly TimeSpan DEFAULT_TTL = TimeSpan.FromDays(7);
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        #endregion

        private readonly IConnectionMultiplexer _conn;
        private readonly IDatabase _redis;

        public SessionRepository(IConnectionMultiplexer connection)
        {
            _conn = connection;
            _redis = connection.GetDatabase();
        }

        public async Task<Session?> Get(string sessionHash)
        {
            var key = BuildKey(sessionHash);
            
            var value = await _redis.StringGetAsync(key);
            if (value.IsNullOrEmpty) 
                return null;

            var doc = JsonSerializer.Deserialize<SessionDocument>(value!, JsonOptions);

            return doc?.ToEntity();
        }

        public async Task<Session?> GetById(Guid sessionId)
        {
            await foreach (var key in EnumerateKeysAsync())
            {
                var value = await _redis.StringGetAsync(key);
                if (value.IsNullOrEmpty) 
                    continue;

                var doc = JsonSerializer.Deserialize<SessionDocument>(value!, JsonOptions);
                if (doc is not null && doc.Id == sessionId)
                    return doc.ToEntity();
            }

            return null;
        }

        public async Task<IEnumerable<Session>> GetByUserId(Guid userId)
        {
            var list = new List<Session>();

            await foreach (var key in EnumerateKeysAsync())
            {
                var value = await _redis.StringGetAsync(key);
                if (value.IsNullOrEmpty) 
                    continue;

                var doc = JsonSerializer.Deserialize<SessionDocument>(value!, JsonOptions);
                if (doc is not null && doc.UserId == userId)
                    list.Add(doc.ToEntity());
            }

            return list.OrderByDescending(s => s.CreatedAt);
        }

        public async Task Set(Session session)
        {
            var key = BuildKey(session.SessionHash);
            var payload = JsonSerializer.Serialize(SessionDocument.ToDocument(session), JsonOptions);

            await _redis.StringSetAsync(key, payload, DEFAULT_TTL);
        }

        public async Task Delete(string sessionHash)
        {
            var key = BuildKey(sessionHash);
            await _redis.KeyDeleteAsync(key);
        }

        public async Task DeleteById(Guid sessionId)
        {
            await foreach (var key in EnumerateKeysAsync())
            {
                var value = await _redis.StringGetAsync(key);
                if (value.IsNullOrEmpty) 
                    continue;

                var doc = JsonSerializer.Deserialize<SessionDocument>(value!, JsonOptions);
                if (doc is not null && doc.Id == sessionId)
                {
                    await _redis.KeyDeleteAsync(key);
                    return;
                }
            }
        }

        #region Private Methods

        private static string BuildKey(string sessionHash) => $"{PREFIX}{sessionHash}";

        private async IAsyncEnumerable<string> EnumerateKeysAsync(int pageSize = 500)
        {
            var pattern = PREFIX + "*";
            foreach (var ep in _conn.GetEndPoints(configuredOnly: true))
            {
                var server = _conn.GetServer(ep);
                foreach (var key in server.Keys(_redis.Database, pattern: pattern, pageSize: pageSize))
                    yield return key;

                await Task.Yield();
            }
        }


        #endregion
    }
}
