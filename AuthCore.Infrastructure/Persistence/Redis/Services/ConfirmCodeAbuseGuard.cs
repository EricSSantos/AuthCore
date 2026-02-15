using AuthCore.Domain.Aggregates.Notifications;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;
using AuthCore.Domain.Core.Settings;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace AuthCore.Infrastructure.Persistence.Redis.Services
{
    /// <summary>Implementa mitigação de abuso para códigos de confirmação usando Redis.</summary>
    public sealed class ConfirmCodeAbuseGuard : IConfirmCodeAbuseGuard
    {
        private static readonly TimeSpan MIN_TTL = TimeSpan.FromSeconds(1);

        private readonly IDatabase _redis;
        private readonly ConfirmCodeAbuseSettings _settings;
        private readonly string _prefix;

        public ConfirmCodeAbuseGuard(
            IConnectionMultiplexer connection,
            IOptions<DatabaseSettings> dbSettings,
            IOptions<SecuritySettings> securitySettings)
        {
            _redis = connection.GetDatabase();
            _settings = securitySettings.Value.ConfirmCodeAbuse;
            _prefix = NormalizePrefix(dbSettings.Value.Redis.KeyPrefix);
        }

        public async Task EnsureIpAllowedAsync(string? ipAddress, NotificationType type, DateTime utcNow)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                return;

            if (_settings.IpPerMinute > 0)
            {
                var key = BuildKey("ip", "global", ipAddress);
                await EnsureRateLimitAsync(key, _settings.IpPerMinute, TimeSpan.FromMinutes(1));
            }

            if (_settings.IpPerMinutePerEndpoint > 0)
            {
                var key = BuildKey("ip", type.ToString().ToLowerInvariant(), ipAddress);
                await EnsureRateLimitAsync(key, _settings.IpPerMinutePerEndpoint, TimeSpan.FromMinutes(1));
            }
        }

        public async Task CheckAndRegisterUserAsync(Guid userId, NotificationType type, DateTime utcNow)
        {
            var typeKey = type.ToString().ToLowerInvariant();

            if (_settings.LockoutMinutes > 0)
            {
                var lockKey = BuildKey("lock", typeKey, userId.ToString());
                if (await _redis.KeyExistsAsync(lockKey))
                    throw new TooManyRequestsException("Muitas tentativas. Aguarde antes de solicitar novo código.");
            }

            if (_settings.ResendCooldownMinutes > 0)
            {
                var cooldownKey = BuildKey("cooldown", typeKey, userId.ToString());
                if (await _redis.KeyExistsAsync(cooldownKey))
                    throw new TooManyRequestsException("Aguarde antes de solicitar novo código.");
            }

            if (_settings.MaxCodesPerWindow > 0 && _settings.WindowMinutes > 0)
            {
                var windowKey = BuildKey("window", typeKey, userId.ToString());
                await EnsureRateLimitAsync(windowKey, _settings.MaxCodesPerWindow, TimeSpan.FromMinutes(_settings.WindowMinutes));
            }

            if (_settings.ResendCooldownMinutes > 0)
            {
                var cooldownKey = BuildKey("cooldown", typeKey, userId.ToString());
                await _redis.StringSetAsync(cooldownKey, "1", TimeSpan.FromMinutes(_settings.ResendCooldownMinutes));
            }
        }

        public async Task RegisterLockoutAsync(Guid userId, NotificationType type, DateTime utcNow)
        {
            if (_settings.LockoutMinutes <= 0)
                return;

            var typeKey = type.ToString().ToLowerInvariant();
            var lockKey = BuildKey("lock", typeKey, userId.ToString());
            await _redis.StringSetAsync(lockKey, "1", TimeSpan.FromMinutes(_settings.LockoutMinutes));
        }

        #region Helpers

        private async Task EnsureRateLimitAsync(string key, int limit, TimeSpan window)
        {
            var current = await _redis.StringIncrementAsync(key);
            if (current == 1)
                await _redis.KeyExpireAsync(key, window);

            if (current > limit)
                throw new TooManyRequestsException("Muitas solicitações. Tente novamente mais tarde.");
        }

        private string BuildKey(string scope, string part1, string part2)
        {
            return $"{_prefix}:confirm-code:abuse:{scope}:{part1}:{part2}";
        }

        private static string NormalizePrefix(string? prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                return "authcore";

            return prefix.Trim().TrimEnd(':');
        }

        #endregion
    }
}
