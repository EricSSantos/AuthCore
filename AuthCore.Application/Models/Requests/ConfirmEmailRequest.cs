using System.Text.Json.Serialization;

namespace AuthCore.Application.Models.Requests
{
    public sealed record ConfirmEmailRequest
    {
        [JsonPropertyName("email")]
        public required string Email { get; set; }

        [JsonPropertyName("code")]
        public required int Code { get; set; }
    }
}
