using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Base;

namespace AuthCore.Domain.Aggregates.Sessions
{
    /// <summary>Representa uma sessão ativa de um usuário autenticado.</summary>
    public sealed class Session : IAggregateRoot
    {
        public string Id { get; private set; } = null!;
        public Guid UserId { get; private set; }
        public DeviceInfo DeviceInfo { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime MaxLifetime { get; private set; }
        public DateTime? RevokedAt { get; private set; }

        #region Constructors

        /// <summary>Operação para criar instância de sessão.</summary>
        /// <param name="id">Identificador da sessão.</param>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="deviceInfo">Dados do dispositivo.</param>
        /// <param name="createdAt">Data de criação da sessão.</param>
        /// <param name="expiresAt">Data de expiração da sessão.</param>
        /// <param name="maxLifetime">Data limite de vida útil.</param>
        /// <param name="revokedAt">Data de revogação da sessão.</param>
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

        /// <summary>Operação para criar sessão.</summary>
        /// <param name="id">Identificador da sessão.</param>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="deviceInfo">Dados do dispositivo.</param>
        /// <param name="ttl">Tempo de expiração da sessão.</param>
        /// <param name="maxLifetime">Tempo máximo de vida útil.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        public static Session Create(
            string id,
            Guid userId,
            DeviceInfo deviceInfo,
            TimeSpan ttl,
            TimeSpan maxLifetime,
            DateTime utcNow)
        {
            return new Session(
                id,
                userId,
                deviceInfo,
                utcNow,
                utcNow.Add(ttl),
                utcNow.Add(maxLifetime),
                revokedAt: null
            );
        }

        /// <summary>Operação para restaurar sessão.</summary>
        /// <param name="id">Identificador da sessão.</param>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="deviceInfo">Dados do dispositivo.</param>
        /// <param name="createdAt">Data de criação da sessão.</param>
        /// <param name="expiresAt">Data de expiração da sessão.</param>
        /// <param name="maxLifetime">Data limite de vida útil.</param>
        /// <param name="revokedAt">Data de revogação da sessão.</param>
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

        /// <summary>Operação para verificar expiração da sessão.</summary>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        public bool IsExpired(DateTime utcNow)
        {
            return utcNow > ExpiresAt || utcNow > MaxLifetime;
        }

        /// <summary>Operação para verificar revogação da sessão.</summary>
        public bool IsRevoked()
        {
            return RevokedAt.HasValue;
        }

        /// <summary>Operação para renovar sessão.</summary>
        /// <param name="ttl">Tempo de expiração da sessão.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        public void Refresh(TimeSpan ttl, DateTime utcNow)
        {
            if (IsRevoked())
                throw new ForbiddenException("Sessão revogada.");

            if (utcNow > MaxLifetime)
                throw new ForbiddenException("A sessão atingiu o tempo máximo permitido.");

            ExpiresAt = utcNow.Add(ttl);
        }

        /// <summary>Operação para revogar sessão.</summary>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        public void Revoke(DateTime utcNow)
        {
            if (IsRevoked())
                return;

            RevokedAt = utcNow;
        }

        #region Validation

        /// <summary>Operação para validar sessão.</summary>
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

        #endregion
    }
}
