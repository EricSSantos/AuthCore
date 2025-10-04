namespace AuthCore.Domain.Settings
{
    public class SecuritySettings
    {
        public JwtSettings Jwt { get; set; } = new();
        public SecurityKeysSettings Keys { get; set; } = new();
        public LifetimeSettings Lifetime { get; set; } = new();
    }

    public class JwtSettings
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }

    public class SecurityKeysSettings
    {
        public SymmetricKeySettings Symmetric { get; set; } = new();
        public AsymmetricKeySettings Asymmetric { get; set; } = new();
    }

    public class SymmetricKeySettings
    {
        public string PrivateKey { get; set; } = string.Empty;
    }

    public class AsymmetricKeySettings
    {
        public string PrivateKeyPath { get; set; } = string.Empty;
        public string PublicKeyPath { get; set; } = string.Empty;
    }

    public class LifetimeSettings
    {
        public int ExpirationInMinutes { get; set; } = 15;
        public int ExpirationInDays { get; set; } = 7;
    }
}
