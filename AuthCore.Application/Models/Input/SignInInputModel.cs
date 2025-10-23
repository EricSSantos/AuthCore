using System.Text.Json.Serialization;

namespace AuthCore.Application.Models.Input
{
    public sealed record SignInInputModel
    {
        [JsonPropertyName("email")]
        public required string Email { get; init; }

        [JsonPropertyName("password")]
        public required string Password { get; init; }
    }
}
