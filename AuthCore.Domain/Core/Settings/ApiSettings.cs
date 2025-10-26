namespace AuthCore.Domain.Core.Settings
{
    /// <summary>
    /// Representa as configurações gerais da API, incluindo informações de versão.
    /// </summary>
    public class ApiSettings
    {
        /// <summary>
        /// Contém as versões disponíveis da API e suas respectivas configurações.
        /// </summary>
        public Dictionary<string, ApiVersionSettings> Versions { get; set; } = new();
    }

    /// <summary>
    /// Define as informações de configuração para uma versão específica da API.
    /// </summary>
    public class ApiVersionSettings
    {
        /// <summary>
        /// Nome identificador da versão da API.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição curta da versão da API.
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
