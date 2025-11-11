using System.Text.Json.Serialization;

namespace AuthCore.Application.Models.Requests
{
    public sealed record ResetPasswordRequest
    {
        [JsonIgnore]
        public string Email { get; set; } = string.Empty;
        
        [JsonPropertyName("code")]
        public required int Code { get; init; }

        [JsonPropertyName("new_password")]
        public required string NewPassword { get; init; }

        [JsonPropertyName("confirm_new_password")]
        public required string ConfirmNewPassword { get; init; }
    }
}
