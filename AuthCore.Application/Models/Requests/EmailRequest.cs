using System.Text.Json.Serialization;

namespace AuthCore.Application.Models.Requests
{
    public sealed record EmailRequest
    {
        [JsonPropertyName("email")]
        public required string Email { get; set; }
    }
}
