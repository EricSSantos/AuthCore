using AuthCore.Domain.Aggregates.Notifications;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;
using AuthCore.Domain.Core.Settings;
using System.Text.Json;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace AuthCore.Infrastructure.Persistence.Redis.Services
{
    /// <summary>Implementa mitigação de abuso para códigos de confirmação usando Redis.</summary>
    public sealed class ConfirmCodeAbuseGuard : IConfirmCodeAbuseGuard
    {
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
            var ip = NormalizeIp(ipAddress);
            if (string.IsNullOrWhiteSpace(ip))
                return;

            if (_settings.IpPerMinute > 0)
                await EnsureIpRateLimitAsync(ip, "global", _settings.IpPerMinute, TimeSpan.FromMinutes(1), utcNow);

            if (_settings.IpPerMinutePerEndpoint > 0)
                await EnsureIpRateLimitAsync(ip, MapTypeKey(type), _settings.IpPerMinutePerEndpoint, TimeSpan.FromMinutes(1), utcNow);
        }

        public async Task CheckAndRegisterUserAsync(Guid userId, NotificationType type, DateTime utcNow)
        {
            var typeKey = MapTypeKey(type);
            var key = BuildUserKey(typeKey, userId);
            var now = new DateTimeOffset(utcNow).ToUnixTimeSeconds();
            var state = await GetUserStateAsync(key);
            var cooldownUntil = state.CooldownUntil;
            var windowCount = state.WindowCount;
            var windowReset = state.WindowReset;
            var lockUntil = state.LockUntil;

            if (_settings.LockoutMinutes > 0 && lockUntil > now)
                throw new TooManyRequestsException("Muitas tentativas. Aguarde antes de solicitar novo código.");

            if (_settings.ResendCooldownMinutes > 0 && cooldownUntil > now)
                throw new TooManyRequestsException("Aguarde antes de solicitar novo código.");

            if (_settings.MaxCodesPerWindow > 0 && _settings.WindowMinutes > 0)
            {
                if (windowReset <= now)
                {
                    windowCount = 0;
                    windowReset = now + (_settings.WindowMinutes * 60L);
                }

                if (windowCount + 1 > _settings.MaxCodesPerWindow)
                    throw new TooManyRequestsException("Muitas solicitações. Tente novamente mais tarde.");

                windowCount += 1;
            }

            if (_settings.ResendCooldownMinutes > 0)
                cooldownUntil = now + (_settings.ResendCooldownMinutes * 60L);

            state = state with
            {
                CooldownUntil = cooldownUntil,
                WindowCount = windowCount,
                WindowReset = windowReset,
                LockUntil = lockUntil
            };

            await SetUserStateAsync(key, state);
            await _redis.KeyExpireAsync(key, ComputeUserTtlSeconds(now, cooldownUntil, windowReset, lockUntil));
        }

        public async Task RegisterLockoutAsync(Guid userId, NotificationType type, DateTime utcNow)
        {
            if (_settings.LockoutMinutes <= 0)
                return;

            var typeKey = MapTypeKey(type);
            var key = BuildUserKey(typeKey, userId);
            var now = new DateTimeOffset(utcNow).ToUnixTimeSeconds();
            var lockUntil = now + (_settings.LockoutMinutes * 60L);

            var state = await GetUserStateAsync(key);
            state = state with { LockUntil = lockUntil };

            await SetUserStateAsync(key, state);
            await _redis.KeyExpireAsync(key, TimeSpan.FromMinutes(_settings.LockoutMinutes));
        }

        #region Helpers

        private async Task EnsureIpRateLimitAsync(string ip, string scope, int limit, TimeSpan window, DateTime utcNow)
        {
            var key = BuildIpKey(ip);
            var now = new DateTimeOffset(utcNow).ToUnixTimeSeconds();

            var state = await GetIpStateAsync(key);
            state.Counts.TryGetValue(scope, out var count);
            state.Resets.TryGetValue(scope, out var reset);

            if (reset <= now)
            {
                count = 0;
                reset = now + (long)window.TotalSeconds;
            }

            if (count + 1 > limit)
                throw new TooManyRequestsException("Muitas solicitações. Tente novamente mais tarde.");

            count += 1;

            state.Counts[scope] = count;
            state.Resets[scope] = reset;

            await SetIpStateAsync(key, state);
            await _redis.KeyExpireAsync(key, TimeSpan.FromSeconds(Math.Max(1, reset - now)));
        }

        private string BuildIpKey(string ip)
        {
            return $"{_prefix}:confirm-code:rate-limit:ip:{ip}";
        }

        private string BuildUserKey(string typeKey, Guid userId)
        {
            return $"{_prefix}:confirm-code:rate-limit:user:{typeKey}:{userId}";
        }

        private static string? NormalizeIp(string? ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return "127.0.0.1";

            if (ip == "::1")
                return "127.0.0.1";

            return ip;
        }

        private static string NormalizePrefix(string? prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                return "authcore";

            return prefix.Trim().TrimEnd(':');
        }

        private static string MapTypeKey(NotificationType type)
        {
            switch (type)
            {
                case NotificationType.ConfirmEmail:
                    return "confirm-email";
                case NotificationType.ForgotPassword:
                    return "forgot-password";
                default:
                    return type.ToString().ToLowerInvariant();
            }
        }

        private static TimeSpan ComputeUserTtlSeconds(long now, long cooldownUntil, long windowReset, long lockUntil)
        {
            var ttl = Math.Max(cooldownUntil, Math.Max(windowReset, lockUntil)) - now;
            if (ttl <= 0)
                return TimeSpan.FromSeconds(1);
            return TimeSpan.FromSeconds(ttl);
        }

        private async Task<UserState> GetUserStateAsync(string key)
        {
            var value = await _redis.StringGetAsync(key);
            if (value.IsNullOrEmpty)
                return UserState.Empty;

            try
            {
                var state = JsonSerializer.Deserialize<UserState>(value!);
                return state ?? UserState.Empty;
            }
            catch
            {
                return UserState.Empty;
            }
        }

        private Task SetUserStateAsync(string key, UserState state)
        {
            var json = JsonSerializer.Serialize(state);
            return _redis.StringSetAsync(key, json);
        }

        private async Task<IpState> GetIpStateAsync(string key)
        {
            var value = await _redis.StringGetAsync(key);
            if (value.IsNullOrEmpty)
                return IpState.Empty;

            try
            {
                var state = JsonSerializer.Deserialize<IpState>(value!);
                return state ?? IpState.Empty;
            }
            catch
            {
                return IpState.Empty;
            }
        }

        private Task SetIpStateAsync(string key, IpState state)
        {
            var json = JsonSerializer.Serialize(state);
            return _redis.StringSetAsync(key, json);
        }

        #endregion

        #region State Models

        private sealed record IpState(Dictionary<string, long> Counts, Dictionary<string, long> Resets)
        {
            public static readonly IpState Empty = new(new Dictionary<string, long>(), new Dictionary<string, long>());
        }

        private sealed record UserState(long CooldownUntil, long WindowCount, long WindowReset, long LockUntil)
        {
            public static readonly UserState Empty = new(0, 0, 0, 0);
        }

        #endregion
    }
}
