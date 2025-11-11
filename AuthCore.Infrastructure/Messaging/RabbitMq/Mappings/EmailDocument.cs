using AuthCore.Domain.Aggregates.MessagingAggregate;
using System.Text.Json.Serialization;

namespace AuthCore.Infrastructure.Messaging.RabbitMq.Mappings
{
    public sealed class EmailDocument
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("to")]
        public string To { get; init; } = string.Empty;

        [JsonPropertyName("full_name")]
        public string FullName { get; init; } = string.Empty;

        [JsonPropertyName("type")]
        public MessagingType Type { get; init; }

        [JsonPropertyName("payload")]
        public object? Payload { get; init; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; init; }
    }
}
