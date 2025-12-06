namespace AuthCore.Domain.Core.Settings
{
    /// <summary>
    /// Define origens permitidas para configuração de CORS.
    /// </summary>
    public sealed class CorsSettings
    {
        /// <summary>
        /// Lista origens permitidas no desenvolvimento.
        /// </summary>
        public string[] DevelopmentOrigins { get; set; } = [];

        /// <summary>
        /// Lista origens permitidas na produção.
        /// </summary>
        public string[] ProductionOrigins { get; set; } = [];
    }
}
