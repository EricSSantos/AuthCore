using AuthCore.Domain.Entities;
using AuthCore.Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace AuthCore.Infrastructure.Adapters.Sessions.Documents
{
    internal sealed class SessionDocument
    {
        [JsonPropertyName("sid")]
        public Guid Sid { get; set; }

        [JsonPropertyName("sub")]
        public Guid Sub { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("expires_at")]
        public DateTimeOffset ExpiresAt { get; set; }

        [JsonPropertyName("last_used_at")]
        public DateTimeOffset? LastUsedAt { get; set; }

        [JsonPropertyName("device")]
        public DeviceDocument Device { get; set; } = default!;

        public static SessionDocument ToDocument(Session session) => new()
        {
            Sid = session.Id,
            Sub = session.UserId,
            RefreshToken = session.RefreshToken,
            CreatedAt = session.CreatedAt,
            ExpiresAt = session.ExpiresAt,
            LastUsedAt = session.LastUsedAt,
            Device = new DeviceDocument
            {
                Ip = session.DeviceInfo.Ip,
                Platform = session.DeviceInfo.Platform,
                Browser = session.DeviceInfo.Browser,
                Location = session.DeviceInfo.Location
            }
        };

        public Session ToEntity()
        {
            var deviceInfo = DeviceInfo.Create(
                Device.Ip,
                Device.Platform,
                Device.Browser,
                Device.Location
            );

            var session = Session.Create(Sub, deviceInfo, RefreshToken);

            typeof(Entity).GetProperty("Id")!.SetValue(session, Sid);
            typeof(Session).GetProperty("CreatedAt")!.SetValue(session, CreatedAt);
            typeof(Session).GetProperty("ExpiresAt")!.SetValue(session, ExpiresAt);
            typeof(Session).GetProperty("LastUsedAt")!.SetValue(session, LastUsedAt);

            return session;
        }
    }
}
