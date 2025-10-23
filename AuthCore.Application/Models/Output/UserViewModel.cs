using System.Text.Json.Serialization;

namespace AuthCore.Application.Models.Output
{
    public sealed record UserViewModel
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("email")]
        public string Email { get; init; } = string.Empty;

        [JsonPropertyName("full_name")]
        public string FullName { get; init; } = string.Empty;

        [JsonPropertyName("role")]
        public string Role { get; init; } = string.Empty;
    }
}
