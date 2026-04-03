namespace AuthCore.Domain.Core.Settings
{
    /// <summary>Representa configurações gerais de segurança da aplicação.</summary>
    public sealed class SecuritySettings
    {
        public JwtSettings Jwt { get; set; } = new();
        public SessionSettings Session { get; set; } = new();
        public CookieSettings Cookies { get; set; } = new();
        public AbuseSettings Abuse { get; set; } = new();
        public LoginAttemptsSettings LoginAttempts { get; set; } = new();
        public ConfirmCodeSettings ConfirmCode { get; set; } = new();
        public ConfirmCodeAbuseSettings ConfirmCodeAbuse { get; set; } = new();
        public SecurityKeysSettings Keys { get; set; } = new();
    }

    /// <summary>Representa configurações do JWT.</summary>
    public sealed class JwtSettings
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiresInMinutes { get; set; } = 30;
    }

    /// <summary>Representa configurações da sessão do usuário.</summary>
    public sealed class SessionSettings
    {
        public int ExpiresInDays { get; set; } = 7;
        public int MaxLifetimeInDays { get; set; } = 30;
        public int MaxSessionsPerUser { get; set; } = 4;
    }

    /// <summary>Representa configurações de tentativas de login.</summary>
    public sealed class LoginAttemptsSettings
    {
        public int MaxAttempts { get; set; } = 5;
        public int LockDurationMinutes { get; set; } = 15;
    }

    /// <summary>Representa configurações de códigos de confirmação.</summary>
    public sealed class ConfirmCodeSettings
    {
        public int ExpiresInMinutes { get; set; } = 10;
        public int MaxAttempts { get; set; } = 5;
    }

    /// <summary>Representa configurações de mitigação de abuso para códigos de confirmação.</summary>
    public sealed class ConfirmCodeAbuseSettings
    {
        public int ResendCooldownMinutes { get; set; } = 2;
        public int WindowMinutes { get; set; } = 1440;
        public int MaxCodesPerWindow { get; set; } = 3;
        public int IpPerMinute { get; set; } = 60;
        public int IpPerMinutePerEndpoint { get; set; } = 20;
    }

    /// <summary>Representa configurações de cookies de segurança.</summary>
    public sealed class CookieSettings
    {
        public string SessionKey { get; set; } = "__Host-session";
        public string AccessTokenKey { get; set; } = "__Host-access_token";
    }

    /// <summary>Configurações globais de mitigação de abuso.</summary>
    public sealed class AbuseSettings
    {
        public int SignInIpPerMinute { get; set; } = 20;
        public int SignInEmailPerMinute { get; set; } = 10;
        public int RefreshPerMinute { get; set; } = 60;
    }

    /// <summary>Representa configurações das chaves criptográficas.</summary>
    public sealed class SecurityKeysSettings
    {
        public SymmetricKeySettings Symmetric { get; set; } = new();
        public AsymmetricKeySettings Asymmetric { get; set; } = new();
    }

    /// <summary>Representa configurações da chave simétrica.</summary>
    public sealed class SymmetricKeySettings
    {
        public string PrivateKey { get; set; } = string.Empty;
    }

    /// <summary>Representa configurações das chaves assimétricas.</summary>
    public sealed class AsymmetricKeySettings
    {
        public string PrivateKeyPath { get; set; } = string.Empty;
        public string PublicKeyPath { get; set; } = string.Empty;
    }
}
