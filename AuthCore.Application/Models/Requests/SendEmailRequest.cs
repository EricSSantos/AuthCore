using System.Text.Json.Serialization;
using AuthCore.Domain.Aggregates.MessagingAggregate;

namespace AuthCore.Application.Models.Requests
{
    public sealed record SendEmailRequest
    {
        [JsonPropertyName("email")]
        public required string Email { get; set; }

        [JsonPropertyName("type")]
        public required MessagingType Type { get; set; }
    }
}
