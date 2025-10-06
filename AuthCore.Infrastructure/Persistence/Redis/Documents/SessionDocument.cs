using AuthCore.Domain.Entities;
using AuthCore.Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace AuthCore.Infrastructure.Persistence.Redis.Documents
{
    internal sealed class SessionDocument
    {
        #region Properties

        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("user_id")]
        public Guid UserId { get; set; }

        [JsonPropertyName("session")]
        public string Session { get; set; } = string.Empty;

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("expires_at")]
        public DateTimeOffset ExpiresAt { get; set; }

        [JsonPropertyName("last_used_at")]
        public DateTimeOffset? LastUsedAt { get; set; }

        [JsonPropertyName("revoked_at")]
        public DateTimeOffset? RevokedAt { get; set; }

        [JsonPropertyName("device")]
        public DeviceDocument Device { get; set; } = default!;

        #endregion

        #region Conversion

        public static SessionDocument ToDocument(Session session)
        {
            var document = new SessionDocument
            {
                Id = session.Id,
                UserId = session.UserId,
                Session = session.SessionHash,
                RefreshToken = session.RefreshToken,
                CreatedAt = session.CreatedAt,
                ExpiresAt = session.ExpiresAt,
                LastUsedAt = session.LastUsedAt,
                RevokedAt = session.RevokedAt,
                Device = new DeviceDocument
                {
                    Ip = session.DeviceInfo.Ip,
                    Platform = session.DeviceInfo.Platform,
                    Browser = session.DeviceInfo.Browser,
                    Location = session.DeviceInfo.Location
                }
            };

            return document;
        }

        public Session ToEntity()
        {
            var deviceInfo = DeviceInfo.Create(
                Device.Ip,
                Device.Platform,
                Device.Browser,
                Device.Location
            );

            var session = Domain.Entities.Session.Create(UserId, deviceInfo, Session, RefreshToken);

            typeof(Entity).GetProperty("Id")!.SetValue(session, Id);
            typeof(Session).GetProperty("CreatedAt")!.SetValue(session, CreatedAt);
            typeof(Session).GetProperty("ExpiresAt")!.SetValue(session, ExpiresAt);
            typeof(Session).GetProperty("LastUsedAt")!.SetValue(session, LastUsedAt);
            typeof(Session).GetProperty("RevokedAt")!.SetValue(session, RevokedAt);

            return session;
        }

        #endregion
    }
}
