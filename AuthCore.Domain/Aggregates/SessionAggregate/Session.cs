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

        #region Constructors

        private Session(
            string id,
            Guid userId,
            DeviceInfo deviceInfo,
            TimeSpan ttl,
            TimeSpan maxLifetime)
        {
            Id = id;
            UserId = userId;
            DeviceInfo = deviceInfo;
            CreatedAt = DateTime.UtcNow;
            ExpiresAt = CreatedAt.Add(ttl);
            MaxLifetime = CreatedAt.Add(maxLifetime);
            Validate();
        }

        private Session(
            string id,
            Guid userId,
            DeviceInfo deviceInfo,
            DateTime createdAt,
            DateTime expiresAt,
            DateTime maxLifetime)
        {
            Id = id;
            UserId = userId;
            DeviceInfo = deviceInfo;
            CreatedAt = createdAt;
            ExpiresAt = expiresAt;
            MaxLifetime = maxLifetime;
            Validate();
        }

        #endregion

        #region Factory

        /// <summary>
        /// Cria uma sessão com tempos de expiração definidos.
        /// </summary>
        /// <param name="id">Identificador da sessão.</param>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="deviceInfo">Informações do dispositivo.</param>
        /// <param name="ttl">Tempo até expiração.</param>
        /// <param name="maxLifetime">Tempo máximo de vida.</param>
        /// <returns>Instância criada de <see cref="Session"/>.</returns>
        public static Session Create(
            string id,
            Guid userId,
            DeviceInfo deviceInfo,
            TimeSpan ttl,
            TimeSpan maxLifetime)
        {
            return new Session(id, userId, deviceInfo, ttl, maxLifetime);
        }

        /// <summary>
        /// Restaura uma sessão previamente persistida.
        /// </summary>
        /// <param name="id">Identificador da sessão.</param>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="deviceInfo">Informações do dispositivo.</param>
        /// <param name="createdAt">Data de criação.</param>
        /// <param name="expiresAt">Data de expiração.</param>
        /// <param name="maxLifetime">Tempo máximo de vida.</param>
        /// <returns>Instância restaurada de <see cref="Session"/>.</returns>
        public static Session Restore(
            string id,
            Guid userId,
            DeviceInfo deviceInfo,
            DateTime createdAt,
            DateTime expiresAt,
            DateTime maxLifetime)
        {
            return new Session(id, userId, deviceInfo, createdAt, expiresAt, maxLifetime);
        }

        #endregion

        #region Behavior

        /// <summary>
        /// Verifica se a sessão está expirada.
        /// </summary>
        /// <returns>True quando a sessão expirou.</returns>
        public bool IsExpired()
        {
            var now = DateTime.UtcNow;
            return now > ExpiresAt || now > MaxLifetime;
        }

        /// <summary>
        /// Renova a expiração da sessão quando permitido.
        /// </summary>
        /// <param name="ttl">Tempo adicional até a próxima expiração.</param>
        /// <exception cref="ForbiddenException">Lançada quando excede o tempo máximo de vida.</exception>
        public void Refresh(TimeSpan ttl)
        {
            if (DateTime.UtcNow > MaxLifetime)
                throw new ForbiddenException("A sessão atingiu o tempo máximo permitido.");

            ExpiresAt = DateTime.UtcNow.Add(ttl);
        }

        #endregion

        #region Validation

        /// <summary>
        /// Valida os dados obrigatórios da sessão.
        /// </summary>
        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Id))
                throw new BadRequestException("O identificador da sessão é obrigatório.");
            if (UserId == Guid.Empty)
                throw new BadRequestException("O identificador do usuário é obrigatório.");
            if (DeviceInfo is null)
                throw new BadRequestException("As informações do dispositivo são obrigatórias.");
            if (ExpiresAt <= CreatedAt)
                throw new BadRequestException("A data de expiração deve ser maior que a data de criação.");
            if (MaxLifetime <= CreatedAt)
                throw new BadRequestException("O tempo máximo de vida útil deve ser maior que a data de criação.");
            if (ExpiresAt > MaxLifetime)
                throw new BadRequestException("A expiração não pode ultrapassar o tempo máximo de vida útil.");
        }

        #endregion
    }
}
