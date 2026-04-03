using System.Text.Json.Serialization;

namespace AuthCore.Application.Models.Requests
{
    /// <summary>Representa dados com e-mail.</summary>
    public sealed record EmailRequest
    {
        [JsonPropertyName("email")]
        public required string Email { get; set; }
    }
}
