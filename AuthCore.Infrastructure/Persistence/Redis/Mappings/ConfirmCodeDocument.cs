using AuthCore.Domain.Aggregates.ConfirmCodeAggregate;
using System.Text.Json.Serialization;

namespace AuthCore.Infrastructure.Persistence.Redis.Mappings
{
    public sealed class ConfirmCodeDocument
    {
        [JsonPropertyName("code")]
        public int Code { get; init; } = default!;

        [JsonPropertyName("type")]
        public CodeType Type { get; init; } = default!;

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; init; }
    }
}
