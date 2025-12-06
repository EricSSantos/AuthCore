namespace AuthCore.Domain.Core.Settings
{
    /// <summary>
    /// Define configurações gerais da API, incluindo versões.
    /// </summary>
    public class ApiSettings
    {
        /// <summary>
        /// Armazena as versões disponíveis da API.
        /// </summary>
        public Dictionary<string, ApiVersionSettings> Versions { get; set; } = new();
    }

    /// <summary>
    /// Define configurações de uma versão específica da API.
    /// </summary>
    public class ApiVersionSettings
    {
        /// <summary>
        /// Nome identificador da versão.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Define uma descrição curta da versão.
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
