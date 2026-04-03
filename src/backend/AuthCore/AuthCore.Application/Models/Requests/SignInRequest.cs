using System.Text.Json.Serialization;

namespace AuthCore.Application.Models.Requests
{
    /// <summary>Representa dados para autenticação do usuário.</summary>
    public sealed record SignInRequest
    {
        [JsonPropertyName("email")]
        public required string Email { get; set; }

        [JsonPropertyName("password")]
        public required string Password { get; set; }
    }
}
