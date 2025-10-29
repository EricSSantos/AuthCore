using System.Text.Json.Serialization;
using AuthCore.Domain.Aggregates.EmailAggregate;

namespace AuthCore.Application.Models.Requests
{
    public sealed record SendEmailRequest
    {
        [JsonPropertyName("email")]
        public required string Email { get; set; }
        [JsonPropertyName("type")]
        public required EmailType Type { get; set; }
    }
}
