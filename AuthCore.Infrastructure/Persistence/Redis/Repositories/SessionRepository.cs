using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Domain.Commons.Interfaces.Helpers;
using AuthCore.Infrastructure.Persistence.Redis.Mappings;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace AuthCore.Infrastructure.Persistence.Redis.Repositories
{
    public sealed class SessionRepository : ISessionRepository
    {
        #region Constants

        private const string ROOT = "sessions:";
        private const string SESSION_DOCUMENT = ROOT + "data:";
        private const string SESSION_ID_IDX = ROOT + "id:";
        private const string USER_ID_IDX = "user:";

        private static readonly TimeSpan DEFAULT_TTL = TimeSpan.FromDays(7);

        #endregion

        private readonly IDistributedCache _cache;
        private readonly IJsonSerializer _serializer;
        private readonly IDatabase _redis;

        public SessionRepository(
            IDistributedCache cache,
            IJsonSerializer serializer,
            IConnectionMultiplexer connection)
        {
            _cache = cache;
            _serializer = serializer;
            _redis = connection.GetDatabase();
        }

        public async Task<Session?> Get(string sessionHash)
        {
            var key = BuildKey(sessionHash);
            var json = await _cache.GetStringAsync(key);

            if (string.IsNullOrWhiteSpace(json))
                return null;

            var document = _serializer.Deserialize<SessionDocument>(json);
            return document?.ToEntity();
        }

        public async Task<Session?> GetById(Guid sessionId)
        {
            var mapKey = BuildSessionIdIndexKey(sessionId);
            var sessionHash = await _cache.GetStringAsync(mapKey);

            if (string.IsNullOrWhiteSpace(sessionHash))
                return null;

            return await Get(sessionHash);
        }

        public async Task<IEnumerable<Session>> GetByUserId(Guid userId)
        {
            var indexKey = BuildUserIndexKey(userId);
            var hashes = await _redis.SetMembersAsync(indexKey);
            var sessions = new List<Session>();

            foreach (var hash in hashes)
            {
                var sessionKey = BuildKey(hash!);
                var json = await _cache.GetStringAsync(sessionKey);

                if (string.IsNullOrWhiteSpace(json))
                    continue;

                var document = _serializer.Deserialize<SessionDocument>(json);
                if (document != null)
                    sessions.Add(document.ToEntity());
            }

            return sessions;
        }

        public async Task Set(Session session)
        {
            var document = SessionDocument.ToDocument(session);
            var json = _serializer.Serialize(document);

            var sessionKey = BuildKey(session.SessionHash);
            var sessionIdKey = BuildSessionIdIndexKey(session.Id);
            var userIndexKey = BuildUserIndexKey(session.UserId);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = DEFAULT_TTL
            };

            await _cache.SetStringAsync(sessionKey, json, options);

            await _redis.SetAddAsync(userIndexKey, session.SessionHash);
            await _redis.KeyExpireAsync(userIndexKey, DEFAULT_TTL);

            await _cache.SetStringAsync(sessionIdKey, session.SessionHash, options);
        }

        public async Task Delete(string sessionHash)
        {
            var sessionKey = BuildKey(sessionHash);
            var json = await _cache.GetStringAsync(sessionKey);

            if (!string.IsNullOrWhiteSpace(json))
            {
                var document = _serializer.Deserialize<SessionDocument>(json);
                if (document != null)
                {
                    var userIndexKey = BuildUserIndexKey(document.UserId);
                    var sessionIdKey = BuildSessionIdIndexKey(document.Id);

                    await _redis.SetRemoveAsync(userIndexKey, sessionHash);
                    await _cache.RemoveAsync(sessionIdKey);
                }
            }

            await _cache.RemoveAsync(sessionKey);
        }

        public async Task DeleteById(Guid sessionId)
        {
            var sessionIdKey = BuildSessionIdIndexKey(sessionId);
            var sessionHash = await _cache.GetStringAsync(sessionIdKey);

            if (string.IsNullOrWhiteSpace(sessionHash))
                return;

            await Delete(sessionHash);
        }

        #region Private Methods

        private static string BuildKey(string hash)
            => $"{SESSION_DOCUMENT}{hash}";

        private static string BuildSessionIdIndexKey(Guid sessionId)
            => $"{SESSION_ID_IDX}{sessionId}";

        private static string BuildUserIndexKey(Guid userId)
            => $"{USER_ID_IDX}{userId}";

        #endregion
    }
}
