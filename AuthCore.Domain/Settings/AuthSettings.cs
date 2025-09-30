namespace AuthCore.Domain.Settings
{
    public class AuthSettings
    {
        public JwtSettings Jwt { get; set; } = new();
    }

    public class JwtSettings
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpirationInMinutes { get; set; }
        public int ExpirationInDays { get; set; }
    }
}
