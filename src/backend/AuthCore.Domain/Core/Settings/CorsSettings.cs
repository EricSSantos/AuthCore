namespace AuthCore.Domain.Core.Settings
{
    /// <summary>Representa configurações de CORS.</summary>
    public sealed class CorsSettings
    {
        public string[] DevelopmentOrigins { get; set; } = [];
        public string[] ProductionOrigins { get; set; } = [];
    }
}
