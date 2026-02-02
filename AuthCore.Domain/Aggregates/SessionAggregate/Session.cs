using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Base;

namespace AuthCore.Domain.Aggregates.SessionAggregate
{
    /// <summary>
    /// Representa uma sessão ativa de um usuário autenticado.
    /// </summary>
    public sealed class Session : IAggregateRoot
    {
        public string Id { get; private set; } = null!;
        public Guid UserId { get; private set; }
        public DeviceInfo DeviceInfo { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime MaxLifetime { get; private set; }
        public DateTime? RevokedAt { get; private set; }

        #region Constructor

        private Session(
            string id,
            Guid userId,
            DeviceInfo deviceInfo,
            DateTime createdAt,
            DateTime expiresAt,
            DateTime maxLifetime,
            DateTime? revokedAt)
        {
            Id = id;
            UserId = userId;
            DeviceInfo = deviceInfo;
            CreatedAt = createdAt;
            ExpiresAt = expiresAt;
            MaxLifetime = maxLifetime;
            RevokedAt = revokedAt;

            Validate();
        }

        #endregion

        #region Factory

        public static Session Create(
            string id,
            Guid userId,
            DeviceInfo deviceInfo,
            TimeSpan ttl,
            TimeSpan maxLifetime)
        {
            var createdAt = DateTime.UtcNow;

            return new Session(
                id,
                userId,
                deviceInfo,
                createdAt,
                createdAt.Add(ttl),
                createdAt.Add(maxLifetime),
                revokedAt: null
            );
        }

        public static Session Restore(
            string id,
            Guid userId,
            DeviceInfo deviceInfo,
            DateTime createdAt,
            DateTime expiresAt,
            DateTime maxLifetime,
            DateTime? revokedAt)
        {
            return new Session(
                id,
                userId,
                deviceInfo,
                createdAt,
                expiresAt,
                maxLifetime,
                revokedAt
            );
        }

        #endregion


        public bool IsExpired()
        {
            var now = DateTime.UtcNow;
            return now > ExpiresAt || now > MaxLifetime;
        }

        public bool IsRevoked()
        {
            return RevokedAt.HasValue;
        }

        public void Refresh(TimeSpan ttl)
        {
            if (IsRevoked())
                throw new ForbiddenException("Sessão revogada.");

            if (DateTime.UtcNow > MaxLifetime)
                throw new ForbiddenException("A sessão atingiu o tempo máximo permitido.");

            ExpiresAt = DateTime.UtcNow.Add(ttl);
        }

        public void Revoke(DateTime utcNow)
        {
            if (IsRevoked())
                return;

            RevokedAt = utcNow;
        }

        private void Validate()
        {
            List<string> errors = new();

            if (string.IsNullOrWhiteSpace(Id))
                errors.Add("Id inválido.");

            if (UserId == Guid.Empty)
                errors.Add("O identificador do usuário é obrigatório.");

            if (DeviceInfo is null)
                errors.Add("As informações do dispositivo são obrigatórias.");

            if (ExpiresAt <= CreatedAt)
                errors.Add("A data de expiração deve ser maior que a data de criação.");

            if (MaxLifetime <= CreatedAt)
                errors.Add("O tempo máximo de vida útil deve ser maior que a data de criação.");

            if (ExpiresAt > MaxLifetime)
                errors.Add("A expiração não pode ultrapassar o tempo máximo de vida útil.");

            if (errors.Count > 0)
                throw new BadRequestException(errors);
        }
    }
}
