using System.Text.Json.Serialization;

namespace AuthCore.Application.Models.Input
{
    public sealed record ResetPasswordInputModel
    {
        [JsonPropertyName("email")]
        public required string Email { get; init; }

        [JsonPropertyName("code")]
        public required int Code { get; init; }

        [JsonPropertyName("new_password")]
        public required string NewPassword { get; init; }

        [JsonPropertyName("confirm_new_passoword")]
        public required string ConfirmNewPassword { get; init; }
    }
}
