namespace AuthCore.Domain.Core.Settings
{
    /// <summary>Representa configurações gerais da API.</summary>
    public class ApiSettings
    {
        public Dictionary<string, ApiVersionSettings> Versions { get; set; } = new();
    }

    /// <summary>Representa configurações de versão da API.</summary>
    public class ApiVersionSettings
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
