using System.Text.Json.Serialization;

namespace AuthCore.Application.Models.Input
{
    public sealed record ChangePasswordInputModel
    {
        [JsonPropertyName("current_password")]
        public required string CurrentPassword { get; init; }

        [JsonPropertyName("new_password")]
        public required string NewPassword { get; init; }

        [JsonPropertyName("confirm_new_password")]
        public required string ConfirmNewPassword { get; init; }
    }
}
