using System.Text.Json.Serialization;

namespace AuthCore.Application.Models.Input
{
    public sealed record SigInInputModel
    {
        [JsonPropertyName("email")]
        public string Email { get; init; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; init; } = string.Empty;
    }
}
