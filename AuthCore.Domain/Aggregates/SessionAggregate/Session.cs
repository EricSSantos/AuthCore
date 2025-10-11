using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Commons.Exceptions;
using AuthCore.Domain.Shared;

namespace AuthCore.Domain.Aggregates.SessionAggregate
{
    public sealed class Session : Entity
    {
        #region Properties

        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public string SessionHash { get; private set; } = string.Empty;
        public string RefreshTokenHash { get; private set; } = string.Empty;
        public DeviceInfo DeviceInfo { get; private set; } = null!;
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset ExpiresAt { get; private set; }

        #endregion

        #region Constructors

        protected Session() { }

        private Session(
            Guid userId,
            DeviceInfo deviceInfo,
            string sessionHash, string
            refreshToken)
        {
            UserId = userId;
            DeviceInfo = deviceInfo;
            SessionHash = sessionHash;
            RefreshTokenHash = refreshToken;
            CreatedAt = DateTimeOffset.UtcNow;
            ExpiresAt = CreatedAt.Add(TimeSpan.FromDays(7));
        }

        #endregion

        #region Factory

        public static Session Create(
            Guid userId,
            DeviceInfo deviceInfo,
            string sessionHash,
            string refreshToken)
        {
            if (userId == Guid.Empty)
                throw new DomainException("O user_id é obrigatório.");
            if (string.IsNullOrWhiteSpace(sessionHash))
                throw new DomainException("O session_hash é obrigatório.");
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new DomainException("O refresh_token é obrigatório.");
            if (deviceInfo is null)
                throw new DomainException("O device_info é obrigatório.");

            return new Session(
                userId,
                deviceInfo,
                sessionHash,
                refreshToken
            );
        }

        #endregion

        #region Behavior

        public bool IsExpired()
        {
            return DateTimeOffset.UtcNow > ExpiresAt;
        }

        #endregion
    }
}
