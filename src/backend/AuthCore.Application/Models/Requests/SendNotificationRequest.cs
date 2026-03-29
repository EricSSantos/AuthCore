using System.Text.Json.Serialization;
using AuthCore.Domain.Aggregates.Notifications;

namespace AuthCore.Application.Models.Requests
{
    /// <summary>Representa dados para envio de notificação.</summary>
    public sealed record SendNotificationRequest
    {
        [JsonPropertyName("email")]
        public required string Email { get; set; }

        [JsonPropertyName("type")]
        public required NotificationType Type { get; set; }

        [JsonPropertyName("ipAddress")]
        public string? IpAddress { get; set; }
    }
}
