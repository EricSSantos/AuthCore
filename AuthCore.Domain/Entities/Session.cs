using AuthCore.Domain.Exceptions;
using AuthCore.Domain.ValueObjects;

namespace AuthCore.Domain.Entities
{
    public sealed class Session : Entity
    {
        #region Properties

        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public string RefreshToken { get; private set; } = string.Empty;
        public DeviceInfo DeviceInfo { get; private set; } = null!;
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset ExpiresAt { get; private set; }
        public DateTimeOffset? LastUsedAt { get; private set; }
        public DateTimeOffset? RevokedAt { get; private set; }
        public bool Revoked
        {
            get { return RevokedAt.HasValue; }
        }

        #endregion

        #region Constructors

        protected Session() { }

        private Session(Guid userId, DeviceInfo deviceInfo, string refreshToken, TimeSpan lifetime)
        {
            UserId = userId;
            DeviceInfo = deviceInfo;
            RefreshToken = refreshToken;
            CreatedAt = DateTimeOffset.UtcNow;
            ExpiresAt = CreatedAt.Add(lifetime);
        }

        #endregion

        #region Factory

        public static Session Create(Guid userId, DeviceInfo deviceInfo, string refreshToken)
        {
            if (userId == Guid.Empty)
                throw new DomainException("O user_id é obrigatório.");

            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new DomainException("O refresh_token é obrigatório.");

            if (deviceInfo is null)
                throw new DomainException("O device_info é obrigatório.");

            return new Session(userId, deviceInfo, refreshToken, TimeSpan.FromDays(7));
        }

        #endregion

        #region Behavior

        public void Rotate(string newRefreshToken)
        {
            if (string.IsNullOrWhiteSpace(newRefreshToken))
                throw new DomainException("O novo refresh_token é obrigatório.");

            RefreshToken = newRefreshToken;
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(7);
            LastUsedAt = DateTimeOffset.UtcNow;
        }

        public void Revoke()
        {
            if (Revoked)
                return;

            RevokedAt = DateTimeOffset.UtcNow;
        }

        public void Touch()
        {
            LastUsedAt = DateTimeOffset.UtcNow;
        }

        public bool IsValid()
        {
            return !Revoked && DateTimeOffset.UtcNow <= ExpiresAt;
        }

        #endregion
    }
}
