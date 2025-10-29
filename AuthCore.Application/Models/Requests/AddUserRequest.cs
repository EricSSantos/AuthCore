using System.Text.Json.Serialization;

namespace AuthCore.Application.Models.Requests
{
    public sealed record AddUserRequest
    {
        [JsonPropertyName("first_name")]
        public required string FirstName { get; init; }

        [JsonPropertyName("last_name")]
        public required string LastName { get; init; }

        [JsonPropertyName("email")]
        public required string Email { get; init; }

        [JsonPropertyName("password")]
        public required string Password { get; init; }

        [JsonPropertyName("confirm_password")]
        public required string ConfirmPassword { get; init; }
    }
}
