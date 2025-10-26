namespace AuthCore.Domain.Core.Settings
{
    /// <summary>
    /// Define as origens permitidas para o CORS em ambientes de desenvolvimento e produção.
    /// </summary>
    public sealed class CorsSettings
    {
        /// <summary>
        /// Lista de origens permitidas durante o desenvolvimento.
        /// </summary>
        public string[] DevelopmentOrigins { get; set; } = [];

        /// <summary>
        /// Lista de origens permitidas em produção.
        /// </summary>
        public string[] ProductionOrigins { get; set; } = [];
    }
}
