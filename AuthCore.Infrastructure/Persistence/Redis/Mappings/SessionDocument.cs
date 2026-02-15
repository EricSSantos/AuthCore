using AuthCore.Domain.Aggregates.Sessions;
using System.Text.Json.Serialization;

namespace AuthCore.Infrastructure.Persistence.Redis.Mappings
{
    /// <summary>Representa documento Redis de sessão.</summary>
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

        /// <summary>Operação para converter sessão em documento.</summary>
        /// <param name="session">Instância da sessão.</param>
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

        /// <summary>Operação para converter documento em sessão.</summary>
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

        /// <summary>Operação para converter data em Unix time.</summary>
        /// <param name="date">Data a converter.</param>
        private static long ToUnix(DateTime date)
        {
            return new DateTimeOffset(date).ToUnixTimeSeconds();
        }

        /// <summary>Operação para converter data opcional em Unix time.</summary>
        /// <param name="date">Data a converter.</param>
        private static long? ToUnix(DateTime? date)
        {
            return date.HasValue ? new DateTimeOffset(date.Value).ToUnixTimeSeconds() : null;
        }

        /// <summary>Operação para converter Unix time em data.</summary>
        /// <param name="seconds">Valor em segundos.</param>
        private static DateTime FromUnix(long seconds)
        {
            return DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
        }

        /// <summary>Operação para converter Unix time opcional em data.</summary>
        /// <param name="seconds">Valor em segundos.</param>
        private static DateTime? FromUnix(long? seconds)
        {
            return seconds.HasValue ? DateTimeOffset.FromUnixTimeSeconds(seconds.Value).UtcDateTime : null;
        }

        #endregion
    }
}
