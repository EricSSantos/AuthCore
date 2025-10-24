using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Domain.Commons.Interfaces.Persistence;
using AuthCore.Infrastructure.Persistence.Redis.Mappings;

namespace AuthCore.Infrastructure.Persistence.Redis.Repositories
{
    public sealed class SessionRepository : ISessionRepository
    {
        #region Constants

        private const string SESSION_NAMESPACE = "sessions:";
        private const string SESSION_DATA_PREFIX = SESSION_NAMESPACE + "data:";
        private const string SESSION_ID_INDEX = SESSION_NAMESPACE + "id:";
        private const string USER_ID_INDEX = SESSION_NAMESPACE + "user:";
        private static readonly TimeSpan DEFAULT_TTL = TimeSpan.FromDays(7);

        #endregion

        private readonly IRedisContext _redisClient;

        public SessionRepository(IRedisContext redisClient)
        {
            _redisClient = redisClient;
        }

        public async Task<Session?> Get(string sessionHash)
        {
            var sessionKey = BuildSessionDataKey(sessionHash);
            var sessionDocument = await _redisClient.Get<SessionDocument>(sessionKey);

            if (sessionDocument is null)
                return null;

            return sessionDocument.ToEntity();
        }

        public async Task<Session?> GetBySessionId(Guid sessionId)
        {
            var sessionIdKey = BuildSessionIdIndexKey(sessionId);
            var sessionHash = await _redisClient.Get<string>(sessionIdKey);

            if (string.IsNullOrWhiteSpace(sessionHash))
                return null;

            return await Get(sessionHash);
        }

        public async Task<IEnumerable<Session>> GetByUserId(Guid userId)
        {
            var userIndexKey = BuildSessionUserIdIndexKey(userId);
            var sessionHashes = await _redisClient.GetIndexMembers(userIndexKey);

            var sessions = new List<Session>();

            foreach (var sessionHash in sessionHashes)
            {
                var sessionKey = BuildSessionDataKey(sessionHash);
                var document = await _redisClient.Get<SessionDocument>(sessionKey);

                if (document is not null)
                    sessions.Add(document.ToEntity());
            }

            return sessions.OrderByDescending(s => s.CreatedAt);
        }

        public async Task Set(Session session)
        {
            var sessionDocument = SessionDocument.ToDocument(session);

            var sessionKey = BuildSessionDataKey(session.SessionHash);
            var sessionIdKey = BuildSessionIdIndexKey(session.Id);
            var userIndexKey = BuildSessionUserIdIndexKey(session.UserId);

            await _redisClient.Set(sessionKey, sessionDocument, DEFAULT_TTL);
            await _redisClient.AddIndex(userIndexKey, session.SessionHash, DEFAULT_TTL);
            await _redisClient.Set(sessionIdKey, session.SessionHash, DEFAULT_TTL);
        }

        public async Task Delete(string sessionHash)
        {
            var sessionKey = BuildSessionDataKey(sessionHash);
            var sessionDocument = await _redisClient.Get<SessionDocument>(sessionKey);
            if (sessionDocument is not null)
            {
                var userIndexKey = BuildSessionUserIdIndexKey(sessionDocument.UserId);
                var sessionIdKey = BuildSessionIdIndexKey(sessionDocument.Id);

                await _redisClient.RemoveIndex(userIndexKey, sessionHash);
                await _redisClient.Delete(sessionIdKey);
            }

            await _redisClient.Delete(sessionKey);
        }

        public async Task DeleteById(Guid sessionId)
        {
            var sessionIdKey = BuildSessionIdIndexKey(sessionId);
            var sessionHash = await _redisClient.Get<string>(sessionIdKey);
            if (string.IsNullOrWhiteSpace(sessionHash))
                return;

            await Delete(sessionHash);
        }

        #region Private Methods

        private static string BuildSessionDataKey(string sessionHash)
        {
            return $"{SESSION_DATA_PREFIX}{sessionHash}";
        }

        private static string BuildSessionIdIndexKey(Guid sessionId)
        {
            return $"{SESSION_ID_INDEX}{sessionId}";
        }

        private static string BuildSessionUserIdIndexKey(Guid userId)
        {
            return $"{USER_ID_INDEX}{userId}";
        }

        #endregion
    }
}
