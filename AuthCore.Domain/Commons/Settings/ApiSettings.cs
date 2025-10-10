namespace AuthCore.Domain.Commons.Settings
{
    public class ApiSettings
    {
        public Dictionary<string, ApiVersionSettings> Versions { get; set; } = new();
    }

    public class ApiVersionSettings
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
