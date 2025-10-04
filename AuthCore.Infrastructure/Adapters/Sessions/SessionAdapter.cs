using AuthCore.Domain.Entities;
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

        private const string PREFIX = "session:";

        #endregion

        private readonly IDistributedCache _cache;

        public SessionAdapter(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<Session?> GetBySid(Guid sid)
        {
            var json = await _cache.GetStringAsync($"{PREFIX}{sid}");
            if (string.IsNullOrWhiteSpace(json))
                return null;

            var document = JsonSerializer.Deserialize<SessionDocument>(json);
            return document is null ? null : document.ToEntity();
        }

        public async Task<Session?> GetBySubAndDevice(Guid sub, DeviceInfo device)
        {
            var indexKey = $"{PREFIX}index:{sub}";
            var sessionIdsJson = await _cache.GetStringAsync(indexKey);

            if (string.IsNullOrWhiteSpace(sessionIdsJson))
                return null;

            var sessionIds = JsonSerializer.Deserialize<List<Guid>>(sessionIdsJson);
            if (sessionIds is null || sessionIds.Count == 0)
                return null;

            foreach (var sid in sessionIds)
            {
                var json = await _cache.GetStringAsync($"{PREFIX}{sid}");
                if (string.IsNullOrWhiteSpace(json))
                    continue;

                var document = JsonSerializer.Deserialize<SessionDocument>(json);
                if (document is null)
                    continue;

                var entity = document.ToEntity();
                if (entity.UserId == sub && entity.DeviceInfo == device)
                    return entity;
            }

            return null;
        }

        public async Task Add(Session session, TimeSpan? ttl = null)
        {
            var document = SessionDocument.ToDocument(session);
            var json = JsonSerializer.Serialize(document);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl ?? TimeSpan.FromDays(7)
            };

            await _cache.SetStringAsync($"{PREFIX}{session.Id}", json, options);

            // Cria um índice no Redis que vincula o usuário às suas sessões ativas,
            // facilitando futuras consultas e validações de login.
            var index = $"{PREFIX}index:{session.UserId}";
            var existing = await _cache.GetStringAsync(index);

            var sids = string.IsNullOrWhiteSpace(existing)
                ? new List<Guid>()
                : JsonSerializer.Deserialize<List<Guid>>(existing) ?? new();

            if (!sids.Contains(session.Id))
            {
                sids.Add(session.Id);
                var sidsJson = JsonSerializer.Serialize(sids);
                await _cache.SetStringAsync(index, sidsJson, options);
            }
        }

        public async Task Delete(Guid sid)
        {
            await _cache.RemoveAsync($"{PREFIX}{sid}");
        }
    }
}
