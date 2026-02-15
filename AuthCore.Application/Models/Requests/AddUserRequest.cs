using System.Text.Json.Serialization;

namespace AuthCore.Application.Models.Requests
{
    /// <summary>Representa dados para adicionar usuário.</summary>
    public sealed record AddUserRequest
    {
        [JsonPropertyName("first_name")]
        public required string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public required string LastName { get; set; }

        [JsonPropertyName("email")]
        public required string Email { get; set; }

        [JsonPropertyName("password")]
        public required string Password { get; set; }

        [JsonPropertyName("confirm_password")]
        public required string ConfirmPassword { get; set; }
    }
}
