using System.Text.Json.Serialization;

namespace AuthCore.Application.Models.Requests
{
    /// <summary>Representa dados para confirmação de e-mail.</summary>
    public sealed record ConfirmEmailRequest
    {
        [JsonIgnore]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public required int Code { get; set; }
    }
}
