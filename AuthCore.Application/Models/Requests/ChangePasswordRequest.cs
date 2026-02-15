using System.Text.Json.Serialization;

namespace AuthCore.Application.Models.Requests
{
    /// <summary>Representa dados para alteração de senha.</summary>
    public sealed record ChangePasswordRequest
    {
        [JsonPropertyName("current_password")]
        public required string CurrentPassword { get; set; }

        [JsonPropertyName("new_password")]
        public required string NewPassword { get; set; }

        [JsonPropertyName("confirm_new_password")]
        public required string ConfirmNewPassword { get; set; }
    }
}
