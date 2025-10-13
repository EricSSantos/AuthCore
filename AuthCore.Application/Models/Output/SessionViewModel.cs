using System.Text.Json.Serialization;

namespace AuthCore.Application.Models.Output
{
    public sealed record class SessionViewModel
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("ip_address")]
        public string IpAddress { get; init; } = string.Empty;

        [JsonPropertyName("platform")]
        public string Platform { get; init; } = string.Empty;

        [JsonPropertyName("browser")]
        public string Browser { get; init; } = string.Empty;

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; init; }

        [JsonPropertyName("is_current")]
        public bool IsCurrent { get; init; }
    }
}
