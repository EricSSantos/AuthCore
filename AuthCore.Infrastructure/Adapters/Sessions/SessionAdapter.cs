using AuthCore.Domain.Entities;
using AuthCore.Domain.Exceptions;
using AuthCore.Domain.Interfaces.Adapters.Sessions;
using AuthCore.Domain.ValueObjects;
using AuthCore.Infrastructure.Adapters.Sessions.Documents;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AuthCore.Infrastructure.Adapters.Sessions
{
    public sealed class SessionAdapter : ISessionAdapter
    {
        #region Constants

        private const string ROOT = "sessions:";
        private const string INDEX = ROOT + "index:";

        #endregion

        private readonly IDistributedCache _cache;

        public SessionAdapter(IDistributedCache cache)
        {
            _cache = cache;
        }

        #region Reading

        public async Task<Session?> GetBySid(Guid sid)
        {
            var json = await _cache.GetStringAsync($"{ROOT}{sid}");
            if (string.IsNullOrWhiteSpace(json))
                return null;

            var document = JsonSerializer.Deserialize<SessionDocument>(json);
            return document?.ToEntity();
        }

        public async Task<Session?> GetBySubAndDevice(Guid sub, DeviceInfo device)
        {
            var indexKey = $"{INDEX}{sub}";
            var sessionIdsJson = await _cache.GetStringAsync(indexKey);
            if (string.IsNullOrWhiteSpace(sessionIdsJson))
                return null;

            var sessionIds = JsonSerializer.Deserialize<List<Guid>>(sessionIdsJson);
            if (sessionIds is null || sessionIds.Count == 0)
                return null;

            foreach (var sid in sessionIds)
            {
                var session = await GetBySid(sid);
                if (session is not null && session.UserId == sub && session.DeviceInfo == device)
                    return session;
            }

            return null;
        }

        public async Task<Session> Validate(Guid sessionId, Guid userId)
        {
            var session = await GetBySid(sessionId)
                ?? throw new NotFoundException("Sessão não encontrada.");

            if (session.UserId != userId)
                throw new UnauthorizedAccessException("A sessão não pertence ao usuário autenticado.");

            if (!session.IsValid())
                throw new UnauthorizedAccessException("A sessão está expirada ou foi revogada.");

            return session;
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

        public async Task Delete(Guid sid)
        {
            await _cache.RemoveAsync($"{ROOT}{sid}");
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

            await _cache.SetStringAsync($"{ROOT}{session.Id}", json, options);
        }

        private async Task AddToIndex(Session session)
        {
            var indexKey = $"{INDEX}{session.UserId}";
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
