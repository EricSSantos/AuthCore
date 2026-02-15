using System.Text.Json.Serialization;

namespace AuthCore.Application.Models.Requests
{
    /// <summary>Representa dados para confirmação de e-mail.</summary>
    public sealed record ConfirmEmailRequest
    {
        [JsonPropertyName("email")]
        public required string Email { get; set; }

        [JsonPropertyName("code")]
        public required int Code { get; set; }
    }
}
