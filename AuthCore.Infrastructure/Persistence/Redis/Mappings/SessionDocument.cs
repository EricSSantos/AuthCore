using AuthCore.Domain.Aggregates.SessionAggregate;
using System.Text.Json.Serialization;

namespace AuthCore.Infrastructure.Persistence.Redis.Mappings
{
    internal sealed class SessionDocument
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("user_id")]
        public Guid UserId { get; set; }

        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }

        [JsonPropertyName("expires_at")]
        public long ExpiresAt { get; set; }

        [JsonPropertyName("max_lifetime")]
        public long MaxLifetime { get; set; }

        [JsonPropertyName("revoked_at")]
        public long? RevokedAt { get; set; }

        [JsonPropertyName("device")]
        public DeviceDocument Device { get; set; } = default!;

        #region Conversion

        public static SessionDocument ToDocument(Session session)
        {
            return new SessionDocument
            {
                Id = session.Id,
                UserId = session.UserId,
                CreatedAt = ToUnix(session.CreatedAt),
                ExpiresAt = ToUnix(session.ExpiresAt),
                MaxLifetime = ToUnix(session.MaxLifetime),
                RevokedAt = ToUnix(session.RevokedAt),
                Device = new DeviceDocument
                {
                    Ip = session.DeviceInfo.Ip,
                    Platform = session.DeviceInfo.Platform,
                    Browser = session.DeviceInfo.Browser
                }
            };
        }

        public Session ToEntity()
        {
            var deviceInfo = DeviceInfo.Create(Device.Ip, Device.Platform, Device.Browser);

            return Session.Restore(
                id: Id,
                userId: UserId,
                deviceInfo: deviceInfo,
                createdAt: FromUnix(CreatedAt),
                expiresAt: FromUnix(ExpiresAt),
                maxLifetime: FromUnix(MaxLifetime),
                revokedAt: FromUnix(RevokedAt)
            );
        }

        #endregion

        #region Helpers

        private static long ToUnix(DateTime date)
        {
            return new DateTimeOffset(date).ToUnixTimeSeconds();
        }

        private static long? ToUnix(DateTime? date)
        {
            return date.HasValue ? new DateTimeOffset(date.Value).ToUnixTimeSeconds() : null;
        }

        private static DateTime FromUnix(long seconds)
        {
            return DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
        }

        private static DateTime? FromUnix(long? seconds)
        {
            return seconds.HasValue ? DateTimeOffset.FromUnixTimeSeconds(seconds.Value).UtcDateTime : null;
        }

        #endregion
    }
}
