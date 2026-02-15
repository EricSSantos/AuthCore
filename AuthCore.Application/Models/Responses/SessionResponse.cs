using System.Text.Json.Serialization;

namespace AuthCore.Application.Models.Responses
{
    /// <summary>Representa dados de sessão do usuário.</summary>
    public sealed class SessionResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("ip_address")]
        public string IpAddress { get; set; } = string.Empty;

        [JsonPropertyName("platform")]
        public string Platform { get; set; } = string.Empty;

        [JsonPropertyName("browser")]
        public string Browser { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("is_current")]
        public bool IsCurrent { get; set; }
    }
}
