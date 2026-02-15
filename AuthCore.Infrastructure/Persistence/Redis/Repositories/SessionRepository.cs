using AuthCore.Domain.Aggregates.Sessions;
using AuthCore.Domain.Aggregates.Sessions.Interfaces;
using AuthCore.Domain.Core.Settings;
using AuthCore.Infrastructure.Persistence.Redis.Mappings;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace AuthCore.Infrastructure.Persistence.Redis.Repositories
{
    /// <summary>Representa repositório Redis de sessões.</summary>
    public sealed class SessionRepository : ISessionRepository
    {
        #region Constants

        private static readonly JsonSerializerOptions JSON_OPTIONS = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        #endregion

        private readonly IDatabase _redis;
        private readonly string _sessionPrefix;
        private readonly string _userSessionsPrefix;

        /// <summary>Operação para criar instância do repositório de sessões.</summary>
        /// <param name="connection">Conexão com Redis.</param>
        /// <param name="settings">Configurações de banco.</param>
        public SessionRepository(IConnectionMultiplexer connection, IOptions<DatabaseSettings> settings)
        {
            _redis = connection.GetDatabase();
            var prefix = NormalizePrefix(settings.Value.Redis.KeyPrefix);
            _sessionPrefix = $"{prefix}:session:sessions:";
            _userSessionsPrefix = $"{prefix}:session:user-sessions:";
        }

        /// <summary>Operação para obter sessão por identificador.</summary>
        /// <param name="sessionId">Identificador da sessão.</param>
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

        /// <summary>Operação para obter sessões ativas do usuário.</summary>
        /// <param name="userId">Identificador do usuário.</param>
        public async Task<IEnumerable<Session>> GetAllByUserIdAsync(Guid userId)
        {
            var indexKey = BuildUserSessionsKey(userId);
            var sessionIds = await _redis.SetMembersAsync(indexKey);
            if (sessionIds.Length == 0)
                return Enumerable.Empty<Session>();

            var keys = sessionIds.Select(id => (RedisKey)BuildKey(id.ToString())).ToArray();
            var values = await _redis.StringGetAsync(keys);

            var list = new List<Session>(values.Length);
            var staleIds = new List<RedisValue>();

            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                if (value.IsNullOrEmpty)
                {
                    staleIds.Add(sessionIds[i]);
                    continue;
                }

                var doc = JsonSerializer.Deserialize<SessionDocument>(value!, JSON_OPTIONS);
                if (doc is not null)
                    list.Add(doc.ToEntity());
            }

            if (staleIds.Count > 0)
                await _redis.SetRemoveAsync(indexKey, staleIds.ToArray());

            return list.OrderByDescending(x => x.CreatedAt);
        }

        /// <summary>Operação para armazenar sessão.</summary>
        /// <param name="session">Instância da sessão.</param>
        public async Task SetAsync(Session session)
        {
            var key = BuildKey(session.Id);
            var document = SessionDocument.ToDocument(session);
            var payload = JsonSerializer.Serialize(document, JSON_OPTIONS);

            var ttl = session.ExpiresAt - DateTime.UtcNow;
            if (ttl <= TimeSpan.Zero)
                return;

            await _redis.StringSetAsync(key, payload, ttl);
            await _redis.SetAddAsync(BuildUserSessionsKey(session.UserId), session.Id);
        }

        /// <summary>Operação para remover sessão.</summary>
        /// <param name="sessionId">Identificador da sessão.</param>
        public async Task DeleteAsync(string sessionId)
        {
            var key = BuildKey(sessionId);
            var value = await _redis.StringGetAsync(key);
            if (!value.IsNullOrEmpty)
            {
                var doc = JsonSerializer.Deserialize<SessionDocument>(value!, JSON_OPTIONS);
                if (doc is not null)
                    await _redis.SetRemoveAsync(BuildUserSessionsKey(doc.UserId), sessionId);
            }

            await _redis.KeyDeleteAsync(key);
        }

        #region Helpers

        /// <summary>Operação para montar chave da sessão.</summary>
        /// <param name="sessionId">Identificador da sessão.</param>
        private string BuildKey(string sessionId)
        {
            return $"{_sessionPrefix}{sessionId}";
        }

        /// <summary>Operação para montar chave do índice por usuário.</summary>
        /// <param name="userId">Identificador do usuário.</param>
        private string BuildUserSessionsKey(Guid userId)
        {
            return $"{_userSessionsPrefix}{userId}";
        }

        /// <summary>Operação para normalizar prefixo de chave Redis.</summary>
        /// <param name="prefix">Prefixo informado.</param>
        private static string NormalizePrefix(string? prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                return "authcore";

            return prefix.Trim().TrimEnd(':');
        }

        #endregion
    }
}
