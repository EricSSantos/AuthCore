using System.Text.Json.Serialization;

namespace AuthCore.Application.Models.Responses
{
    /// <summary>Representa dados do usuário.</summary>
    public sealed record UserResponse
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("full_name")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;
    }
}
