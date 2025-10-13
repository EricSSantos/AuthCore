namespace AuthCore.Domain.Commons.Settings
{
    /// <summary>
    /// Representa as configurações gerais da API, incluindo suas versões.
    /// </summary>
    public class ApiSettings
    {
        /// <summary>
        /// Dicionário contendo as versões disponíveis da API e suas configurações.
        /// </summary>
        public Dictionary<string, ApiVersionSettings> Versions { get; set; } = new();
    }

    /// <summary>
    /// Define as informações de uma versão específica da API.
    /// </summary>
    public class ApiVersionSettings
    {
        /// <summary>
        /// Nome da versão da API.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição breve da versão da API.
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
