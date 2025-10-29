using System.Text.Json.Serialization;

namespace AuthCore.Application.Models.Requests
{
    public sealed record ForgotPasswordRequest
    {
        [JsonPropertyName("email")]
        public required string Email { get; init; }
    }
}
