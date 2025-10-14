using AuthCore.Domain.Aggregates.EmailAggregate;
using System.Text.Json.Serialization;

namespace AuthCore.Infrastructure.Messaging.RabbitMq.Documents
{
    public sealed class EmailDocument
    {
        [JsonPropertyName("to")]
        public string To { get; init; } = string.Empty;

        [JsonPropertyName("full_name")]
        public string FullName { get; init; } = string.Empty;

        [JsonPropertyName("type")]
        public EmailType Type { get; init; }

        [JsonPropertyName("content")]
        public string Content { get; init; } = string.Empty;

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    }
}
