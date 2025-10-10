using AuthCore.Domain.Commons.Interfaces.Helpers;
using AuthCore.Domain.Commons.Interfaces.Repositories;
using AuthCore.Domain.Entities;
using AuthCore.Infrastructure.Persistence.Redis.Mappings;
using Microsoft.Extensions.Caching.Distributed;

namespace AuthCore.Infrastructure.Persistence.Redis.Repositories
{
    public sealed class SessionRepository : ISessionRepository
    {
        #region Constants

        private const string ROOT = "session:";
        private static readonly TimeSpan DEFAULT_TTL = TimeSpan.FromDays(7);

        #endregion

        private readonly IDistributedCache _cache;
        private readonly IJsonSerializer _serializer;

        public SessionRepository(
            IDistributedCache cache,
            IJsonSerializer serializer)
        {
            _cache = cache;
            _serializer = serializer;
        }

        public async Task<Session?> Get(string session)
        {
            var key = BuildKey(session);
            var json = await _cache.GetStringAsync(key);

            if (string.IsNullOrWhiteSpace(json))
                return null;

            var document = _serializer.Deserialize<SessionDocument>(json);
            return document?.ToEntity();
        }

        public async Task Set(Session session)
        {
            var document = SessionDocument.ToDocument(session);
            var json = _serializer.Serialize(document);
            var key = BuildKey(session.SessionHash);

            await _cache.SetStringAsync(key, json,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = DEFAULT_TTL
                }
            );
        }

        public async Task Delete(string session)
        {
            var key = BuildKey(session);
            await _cache.RemoveAsync(key);
        }

        #region Private Methods

        private static string BuildKey(string hash)
        {
            return $"{ROOT}{hash}";
        }

        #endregion
    }
}
