using AuthCore.Domain.Entities;
using AuthCore.Domain.Interfaces.Security.Sessions;
using AuthCore.Domain.ValueObjects;
using AuthCore.Infrastructure.Persistence.Redis.Documents;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AuthCore.Infrastructure.Security.Sessions
{
    public sealed class SessionService : ISessionStorage
    {
        #region Constants

        private const string ROOT = "session";
        private const string DATA = ROOT + ":data:";
        private const string INDEX = ROOT + ":index:";

        #endregion

        private readonly IDistributedCache _cache;
        private readonly ISessionIdentity _identity;

        public SessionService(IDistributedCache cache, ISessionIdentity identity)
        {
            _cache = cache;
            _identity = identity;
        }

        #region Reading

        public async Task<Session?> GetById(Guid id)
        {
            var json = await _cache.GetStringAsync(DATA + id);
            if (string.IsNullOrWhiteSpace(json))
                return null;

            var document = JsonSerializer.Deserialize<SessionDocument>(json);
            if (document == null)
                return null;

            return document.ToEntity();
        }

        public async Task<Session?> GetBySession(Guid userId, string rawSession)
        {
            var indexKey = INDEX + userId;
            var sessionIdsJson = await _cache.GetStringAsync(indexKey);
            if (string.IsNullOrWhiteSpace(sessionIdsJson))
                return null;

            var sessionIds = JsonSerializer.Deserialize<List<Guid>>(sessionIdsJson);
            if (sessionIds == null || sessionIds.Count == 0)
                return null;

            foreach (var sid in sessionIds)
            {
                var session = await GetById(sid);
                if (session != null && _identity.Verify(rawSession, session.SessionHash))
                    return session;
            }

            return null;
        }

        public async Task<Session?> GetByDevice(Guid sub, DeviceInfo device)
        {
            var indexKey = INDEX + sub;
            var sessionIdsJson = await _cache.GetStringAsync(indexKey);
            if (string.IsNullOrWhiteSpace(sessionIdsJson))
                return null;

            var sessionIds = JsonSerializer.Deserialize<List<Guid>>(sessionIdsJson);
            if (sessionIds == null || sessionIds.Count == 0)
                return null;

            foreach (var sid in sessionIds)
            {
                var session = await GetById(sid);
                if (session != null &&
                    session.UserId == sub &&
                    session.DeviceInfo.Equals(device) &&
                    session.IsValid())
                {
                    return session;
                }
            }

            return null;
        }

        #endregion

        #region Writing

        public async Task Add(Session session)
        {
            await Save(session);
            await AddToIndex(session);
        }

        public async Task Update(Session session)
        {
            await Save(session);
        }

        public async Task Delete(Guid id)
        {
            await _cache.RemoveAsync(DATA + id);
        }

        #endregion

        #region Private Methods

        private async Task Save(Session session)
        {
            var document = SessionDocument.ToDocument(session);
            var json = JsonSerializer.Serialize(document);

            var ttl = session.ExpiresAt - DateTimeOffset.UtcNow;
            if (ttl <= TimeSpan.Zero)
                ttl = TimeSpan.FromDays(7);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            };

            await _cache.SetStringAsync(DATA + session.Id, json, options);
        }

        private async Task AddToIndex(Session session)
        {
            var indexKey = INDEX + session.UserId;
            var existing = await _cache.GetStringAsync(indexKey);

            var sids = string.IsNullOrWhiteSpace(existing)
                ? new List<Guid>()
                : JsonSerializer.Deserialize<List<Guid>>(existing) ?? new List<Guid>();

            if (!sids.Contains(session.Id))
            {
                sids.Add(session.Id);

                var json = JsonSerializer.Serialize(sids);
                var ttl = session.ExpiresAt - DateTimeOffset.UtcNow;

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = ttl
                };

                await _cache.SetStringAsync(indexKey, json, options);
            }
        }

        #endregion
    }
}
