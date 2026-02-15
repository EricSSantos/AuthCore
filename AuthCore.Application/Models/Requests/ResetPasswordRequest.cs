using System.Text.Json.Serialization;

namespace AuthCore.Application.Models.Requests
{
    /// <summary>Representa dados para redefinição de senha.</summary>
    public sealed record ResetPasswordRequest
    {
        [JsonIgnore]
        public string Email { get; set; } = string.Empty;
        
        [JsonPropertyName("code")]
        public required int Code { get; set; }

        [JsonPropertyName("new_password")]
        public required string NewPassword { get; set; }

        [JsonPropertyName("confirm_new_password")]
        public required string ConfirmNewPassword { get; set; }
    }
}
