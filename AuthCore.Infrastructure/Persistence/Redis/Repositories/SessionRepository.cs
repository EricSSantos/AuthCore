using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Domain.Aggregates.SessionAggregate.Interfaces;
using AuthCore.Infrastructure.Persistence.Redis.Mappings;
using StackExchange.Redis;
using System.Text.Json;

namespace AuthCore.Infrastructure.Persistence.Redis.Repositories
{
    public sealed class SessionRepository : ISessionRepository
    {
        #region Constants

        private const string PREFIX = "sessions:";
        private static readonly JsonSerializerOptions JSON_OPTIONS = new()
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

        public async Task<Session?> GetAsync(string sessionId)
        {
            var key = BuildKey(sessionId);
            var value = await _redis.StringGetAsync(key);

            if (value.IsNullOrEmpty)
                return null;

            var doc = JsonSerializer.Deserialize<SessionDocument>(value!, JSON_OPTIONS);
            if (doc is null)
                return null;

            return doc.ToEntity();
        }

        public async Task<IEnumerable<Session>> GetAllByUserIdAsync(Guid userId)
        {
            var list = new List<Session>();

            await foreach (var key in EnumerateKeysAsync())
            {
                var value = await _redis.StringGetAsync(key);
                if (value.IsNullOrEmpty)
                    continue;

                var doc = JsonSerializer.Deserialize<SessionDocument>(value!, JSON_OPTIONS);
                if (doc is not null && doc.UserId == userId)
                    list.Add(doc.ToEntity());
            }

            return list.OrderByDescending(x => x.CreatedAt);
        }

        public async Task SetAsync(Session session)
        {
            var key = BuildKey(session.Id);
            var document = SessionDocument.ToDocument(session);
            var payload = JsonSerializer.Serialize(document, JSON_OPTIONS);
            var ttl = session.ExpiresAt - DateTime.UtcNow;
            await _redis.StringSetAsync(key, payload, ttl);
        }

        public async Task DeleteAsync(string sessionId)
        {
            var key = BuildKey(sessionId);
            await _redis.KeyDeleteAsync(key);
        }

        #region Helpers

        private static string BuildKey(string sessionId)
        {
            return $"{PREFIX}{sessionId}";
        }

        private async IAsyncEnumerable<string> EnumerateKeysAsync(int pageSize = 500)
        {
            var pattern = PREFIX + "*";

            foreach (var endpoint in _conn.GetEndPoints(configuredOnly: true))
            {
                var server = _conn.GetServer(endpoint);

                foreach (var key in server.Keys(_redis.Database, pattern: pattern, pageSize: pageSize))
                    yield return key;

                await Task.Yield();
            }
        }

        #endregion
    }
}
