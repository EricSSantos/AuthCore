using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Shared;

namespace AuthCore.Domain.Aggregates.SessionAggregate
{
    public sealed class Session : Entity
    {
        #region Properties

        public Guid UserId { get; private set; }
        public User User { get; private set; }
        public string SessionHash { get; private set; }
        public string RefreshTokenHash { get; private set; }
        public DeviceInfo DeviceInfo { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }

        #endregion

        #region Constructors

        protected Session() { }

        private Session(
            Guid userId,
            DeviceInfo deviceInfo,
            string sessionHash,
            string refreshToken)
        {
            UserId = userId;
            DeviceInfo = deviceInfo;
            SessionHash = sessionHash;
            RefreshTokenHash = refreshToken;
            CreatedAt = DateTime.UtcNow;
            ExpiresAt = CreatedAt.AddDays(7);
            Validate();
        }

        #endregion

        #region Factory

        public static Session Create(
            Guid userId,
            DeviceInfo deviceInfo,
            string sessionHash,
            string refreshToken)
        {
            return new Session(userId, deviceInfo, sessionHash, refreshToken);
        }

        #endregion

        #region Behavior

        /// <summary>
        /// Indica se a sessão está expirada.
        /// </summary>
        public bool IsExpired()
        {
            return DateTime.UtcNow > ExpiresAt;
        }

        #endregion

        #region Private Methods

        private void Validate()
        {
            var validate = Validator();

            if (UserId == Guid.Empty)
                validate.AddError("O identificador do usuário é obrigatório.");
            if (DeviceInfo is null)
                validate.AddError("As informações do dispositivo são obrigatórias.");
            if (string.IsNullOrWhiteSpace(SessionHash))
                validate.AddError("O a sessão é obrigatório.");
            if (string.IsNullOrWhiteSpace(RefreshTokenHash))
                validate.AddError("O token de atualização é obrigatório.");

            validate.ThrowIfInvalid();
        }

        #endregion
    }
}
