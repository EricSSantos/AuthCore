using AuthCore.Domain.Aggregates.ConfirmCodes;
using AuthCore.Domain.Aggregates.ConfirmCodes.Interfaces;
using AuthCore.Domain.Core.Settings;
using AuthCore.Infrastructure.Persistence.Redis.Mappings;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace AuthCore.Infrastructure.Persistence.Redis.Repositories
{
    /// <summary>Representa repositório Redis de códigos de confirmação.</summary>
    public sealed class ConfirmCodeRepository : IConfirmCodeRepository
    {
        #region Constants

        private static readonly TimeSpan MIN_TTL = TimeSpan.FromSeconds(1);
        private static readonly JsonSerializerOptions JSON_OPTIONS = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        #endregion

        private readonly IDatabase _redis;
        private readonly string _confirmCodePrefix;

        /// <summary>Operação para criar instância do repositório de códigos.</summary>
        /// <param name="connection">Conexão com Redis.</param>
        /// <param name="settings">Configurações de banco.</param>
        public ConfirmCodeRepository(IConnectionMultiplexer connection, IOptions<DatabaseSettings> settings)
        {
            _redis = connection.GetDatabase();
            var prefix = NormalizePrefix(settings.Value.Redis.KeyPrefix);
            _confirmCodePrefix = $"{prefix}:confirm-code:";
        }

        /// <summary>Operação para obter código de confirmação.</summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="type">Tipo do código.</param>
        public async Task<ConfirmCode?> GetAsync(Guid userId, CodeType type)
        {
            var key = BuildKey(userId, type);
            var value = await _redis.StringGetAsync(key);

            if (value.IsNullOrEmpty)
                return null;

            var document = JsonSerializer.Deserialize<ConfirmCodeDocument>(value!, JSON_OPTIONS);
            if (document is null)
                return null;

            return document.ToEntity();
        }

        /// <summary>Operação para armazenar código de confirmação.</summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="confirmCode">Instância do código.</param>
        public async Task SetAsync(Guid userId, ConfirmCode confirmCode)
        {
            var key = BuildKey(userId, confirmCode.Type);

            var document = ConfirmCodeDocument.ToDocument(confirmCode, userId);

            var json = JsonSerializer.Serialize(document, JSON_OPTIONS);
            var ttl = confirmCode.ExpiresAt - DateTime.UtcNow;
            if (ttl <= TimeSpan.Zero)
                ttl = MIN_TTL;

            await _redis.StringSetAsync(key, json, ttl);
        }

        /// <summary>Operação para remover código de confirmação.</summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="type">Tipo do código.</param>
        public async Task DeleteAsync(Guid userId, CodeType type)
        {
            var key = BuildKey(userId, type);
            await _redis.KeyDeleteAsync(key);
        }

        #region Helpers

        /// <summary>Operação para montar chave do código.</summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="type">Tipo do código.</param>
        private string BuildKey(Guid userId, CodeType type)
        {
            return $"{_confirmCodePrefix}{MapTypeKey(type)}:{userId}";
        }

        private static string MapTypeKey(CodeType type)
        {
            switch (type)
            {
                case CodeType.ConfirmEmail:
                    return "confirm-email";
                case CodeType.ForgotPassword:
                    return "forgot-password";
                default:
                    return type.ToString().ToLowerInvariant();
            }
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
