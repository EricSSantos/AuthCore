using AuthCore.Domain.Aggregates.Notifications;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;
using AuthCore.Domain.Core.Settings;
using Microsoft.Extensions.Options;

namespace AuthCore.Infrastructure.Persistence.Redis.Services
{
    /// <summary>Representa proteção de abuso para fluxos de códigos de confirmação.</summary>
    public sealed class ConfirmCodeAbuseGuard : IConfirmCodeAbuseGuard
    {
        private readonly IRateLimitStore _store;
        private readonly ConfirmCodeAbuseSettings _settings;
        private readonly string _prefix;

        /// <summary>Operação para criar instância da proteção de abuso para códigos.</summary>
        /// <param name="store">Armazenamento de rate limiting.</param>
        /// <param name="dbSettings">Configurações de banco de dados.</param>
        /// <param name="securitySettings">Configurações de segurança.</param>
        public ConfirmCodeAbuseGuard(
            IRateLimitStore store,
            IOptions<DatabaseSettings> dbSettings,
            IOptions<SecuritySettings> securitySettings)
        {
            _store = store;
            _settings = securitySettings.Value.ConfirmCodeAbuse;
            _prefix = NormalizePrefix(dbSettings.Value.Redis.KeyPrefix);
        }

        /// <summary>Operação para validar limite de requisições por IP.</summary>
        /// <param name="ipAddress">Endereço IP da requisição.</param>
        /// <param name="type">Tipo da notificação/código.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        public async Task EnsureIpAllowedAsync(string? ipAddress, NotificationType type, DateTime utcNow)
        {
            _ = utcNow;
            var ip = NormalizeIp(ipAddress);
            if (string.IsNullOrWhiteSpace(ip))
                return;

            if (_settings.IpPerMinute > 0)
            {
                await _store.EnsureFixedWindowAsync(
                    key: $"{_prefix}:confirm-code:ip:{ip}:global",
                    limit: _settings.IpPerMinute,
                    window: TimeSpan.FromMinutes(1),
                    errorMessage: "Muitas solicitações. Tente novamente mais tarde.");
            }

            if (_settings.IpPerMinutePerEndpoint > 0)
            {
                await _store.EnsureFixedWindowAsync(
                    key: $"{_prefix}:confirm-code:ip:{ip}:{MapTypeKey(type)}",
                    limit: _settings.IpPerMinutePerEndpoint,
                    window: TimeSpan.FromMinutes(1),
                    errorMessage: "Muitas solicitações. Tente novamente mais tarde.");
            }
        }

        /// <summary>Operação para validar e registrar envio por usuário.</summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="type">Tipo da notificação/código.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        public async Task CheckAndRegisterUserAsync(Guid userId, NotificationType type, DateTime utcNow)
        {
            _ = utcNow;
            var typeKey = MapTypeKey(type);
            var baseKey = $"{_prefix}:confirm-code:user:{typeKey}:{userId}";

            if (_settings.MaxCodesPerWindow > 0 && _settings.WindowMinutes > 0)
            {
                await _store.EnsureFixedWindowAsync(
                    key: $"{baseKey}:window",
                    limit: _settings.MaxCodesPerWindow,
                    window: TimeSpan.FromMinutes(_settings.WindowMinutes),
                    errorMessage: "Muitas solicitações. Tente novamente mais tarde.");
            }

            if (_settings.ResendCooldownMinutes > 0)
            {
                await _store.EnsureCooldownAsync(
                    key: baseKey,
                    cooldown: TimeSpan.FromMinutes(_settings.ResendCooldownMinutes),
                    errorMessage: "Aguarde antes de solicitar novo código.");
            }
        }

        /// <summary>Operação para normalizar endereço IP.</summary>
        /// <param name="ip">Endereço IP informado.</param>
        private static string? NormalizeIp(string? ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return "127.0.0.1";
            if (ip == "::1")
                return "127.0.0.1";
            return ip;
        }

        /// <summary>Operação para normalizar prefixo de chave Redis.</summary>
        /// <param name="prefix">Prefixo informado.</param>
        private static string NormalizePrefix(string? prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                return "authcore";
            return prefix.Trim().TrimEnd(':');
        }

        /// <summary>Operação para mapear tipo de notificação para chave textual.</summary>
        /// <param name="type">Tipo da notificação/código.</param>
        private static string MapTypeKey(NotificationType type)
        {
            return type switch
            {
                NotificationType.ConfirmEmail => "confirm-email",
                NotificationType.ForgotPassword => "forgot-password",
                _ => type.ToString().ToLowerInvariant()
            };
        }
    }
}
