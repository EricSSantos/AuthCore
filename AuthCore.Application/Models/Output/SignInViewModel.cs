using System.Text.Json.Serialization;

namespace AuthCore.Application.Models.Output
{
    public sealed record SignInViewModel
    {
        [JsonPropertyName("session_id")]
        public Guid SessionId { get; init; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; } = string.Empty;

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; init; } = string.Empty;

        public static SignInViewModel ToViewModel(Guid sessionId, string accessToken, string refreshToken)
        {
            return new SignInViewModel
            {
                SessionId = sessionId,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}
