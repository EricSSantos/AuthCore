using System.Text.Json.Serialization;

namespace AuthCore.Application.Models.Requests
{
    public sealed record SignInRequest
    {
        [JsonPropertyName("email")]
        public required string Email { get; init; }

        [JsonPropertyName("password")]
        public required string Password { get; init; }
    }
}
