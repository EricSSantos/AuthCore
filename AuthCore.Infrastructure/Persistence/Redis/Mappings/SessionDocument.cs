using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Domain.Shared;
using System.Text.Json.Serialization;

namespace AuthCore.Infrastructure.Persistence.Redis.Mappings
{
    internal sealed class SessionDocument
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("user_id")]
        public Guid UserId { get; set; }

        [JsonPropertyName("session")]
        public string Session { get; set; } = string.Empty;

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [JsonPropertyName("device")]
        public DeviceDocument Device { get; set; } = default!;

        #region Conversion

        public static SessionDocument ToDocument(Session session)
        {
            var document = new SessionDocument
            {
                Id = session.Id,
                UserId = session.UserId,
                Session = session.SessionHash,
                RefreshToken = session.RefreshTokenHash,
                CreatedAt = session.CreatedAt,
                ExpiresAt = session.ExpiresAt,
                Device = new DeviceDocument
                {
                    Ip = session.DeviceInfo.Ip,
                    Platform = session.DeviceInfo.Platform,
                    Browser = session.DeviceInfo.Browser
                }
            };

            return document;
        }

        public Session ToEntity()
        {
            var deviceInfo = DeviceInfo.Create(
                Device.Ip,
                Device.Platform,
                Device.Browser
            );

            var session = Domain.Aggregates.SessionAggregate.Session.Create(UserId, deviceInfo, Session, RefreshToken);

            typeof(Entity).GetProperty("Id")!.SetValue(session, Id);
            typeof(Session).GetProperty("CreatedAt")!.SetValue(session, CreatedAt);
            typeof(Session).GetProperty("ExpiresAt")!.SetValue(session, ExpiresAt);

            return session;
        }

        #endregion
    }
}
