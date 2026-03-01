using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;
using AuthCore.Domain.Core.Settings;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace AuthCore.Infrastructure.Persistence.Redis.Services
{
    /// <summary>Representa armazenamento Redis para mitigação de abuso e rate limiting.</summary>
    public sealed class RedisRateLimitStore : IRateLimitStore
    {
        private readonly IDatabase _redis;
        private readonly string _prefix;

        /// <summary>Operação para criar instância do armazenamento de rate limiting.</summary>
        /// <param name="connection">Conexão com Redis.</param>
        /// <param name="settings">Configurações de banco de dados.</param>
        public RedisRateLimitStore(IConnectionMultiplexer connection, IOptions<DatabaseSettings> settings)
        {
            _redis = connection.GetDatabase();
            _prefix = NormalizePrefix(settings.Value.Redis.KeyPrefix);
        }

        /// <summary>Operação para validar limite em janela fixa.</summary>
        /// <param name="key">Chave de partição do limite.</param>
        /// <param name="limit">Quantidade máxima permitida na janela.</param>
        /// <param name="window">Duração da janela fixa.</param>
        /// <param name="errorMessage">Mensagem de erro quando o limite for excedido.</param>
        public async Task EnsureFixedWindowAsync(string key, int limit, TimeSpan window, string errorMessage)
        {
            if (limit <= 0)
                return;
            if (window <= TimeSpan.Zero)
                throw new BadRequestException("A janela do rate limit é inválida.");

            var seconds = (int)Math.Ceiling(window.TotalSeconds);
            var bucket = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / seconds;
            var redisKey = BuildKey($"{key}:w{seconds}:b{bucket}");

            var result = await _redis.StringIncrementAsync(redisKey);
            if (result == 1)
                await _redis.KeyExpireAsync(redisKey, TimeSpan.FromSeconds(seconds));

            var current = (long)result;
            if (current > limit)
                throw new TooManyRequestsException(errorMessage);
        }

        /// <summary>Operação para validar cooldown por chave.</summary>
        /// <param name="key">Chave de partição do cooldown.</param>
        /// <param name="cooldown">Tempo mínimo entre operações.</param>
        /// <param name="errorMessage">Mensagem de erro quando o cooldown estiver ativo.</param>
        public async Task EnsureCooldownAsync(string key, TimeSpan cooldown, string errorMessage)
        {
            if (cooldown <= TimeSpan.Zero)
                return;

            var redisKey = BuildKey($"{key}:cooldown");
            var ok = await _redis.StringSetAsync(redisKey, "1", cooldown, When.NotExists);
            if (!ok)
                throw new TooManyRequestsException(errorMessage);
        }

        /// <summary>Operação para montar a chave Redis com prefixo de abuso.</summary>
        /// <param name="suffix">Sufixo da chave.</param>
        private RedisKey BuildKey(string suffix)
        {
            return (RedisKey)$"{_prefix}:abuse:{suffix}";
        }

        /// <summary>Operação para normalizar prefixo de chave Redis.</summary>
        /// <param name="prefix">Prefixo informado.</param>
        private static string NormalizePrefix(string? prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                return "authcore";

            return prefix.Trim().TrimEnd(':');
        }
    }
}
