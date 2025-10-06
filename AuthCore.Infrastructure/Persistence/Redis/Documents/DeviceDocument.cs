using System.Text.Json.Serialization;

namespace AuthCore.Infrastructure.Persistence.Redis.Documents
{
    internal sealed class DeviceDocument
    {
        [JsonPropertyName("ip")]
        public string Ip { get; set; } = string.Empty;

        [JsonPropertyName("platform")]
        public string Platform { get; set; } = string.Empty;

        [JsonPropertyName("browser")]
        public string Browser { get; set; } = string.Empty;

        [JsonPropertyName("location")]
        public string Location { get; set; } = string.Empty;
    }
}
