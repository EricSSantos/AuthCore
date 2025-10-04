using System.Text.Json.Serialization;
using AuthCore.Domain.Entities;

namespace AuthCore.Application.Models.Input
{
    public sealed record AddUserInputModel
    {
        [JsonPropertyName("first_name")]
        public string FirstName { get; init; } = string.Empty;

        [JsonPropertyName("last_name")]
        public string LastName { get; init; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; init; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; init; } = string.Empty;

        [JsonPropertyName("confirm_password")]
        public string ConfirmPassword { get; init; } = string.Empty;

        public User ToEntity(string hashedPassword)
        {
            return User.Create(
                firstName: FirstName,
                lastName: LastName,
                email: Email,
                password: hashedPassword
            );
        }
    }
}
