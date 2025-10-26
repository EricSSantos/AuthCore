using AuthCore.Domain.Core.Interfaces.Base;

namespace AuthCore.Domain.Aggregates.UserAggregate
{
    /// <summary>
    /// Representa o controle de tentativas de login de um usuário,
    /// incluindo bloqueios temporários após falhas consecutivas.
    /// </summary>
    public sealed class LoginAttempts : IValueObject
    {
        #region Constants

        private const int MAX_ATTEMPTS = 5;
        private static readonly TimeSpan LOCK_DURATION = TimeSpan.FromMinutes(15);

        #endregion

        #region Properties

        /// <summary>
        /// Quantidade de tentativas consecutivas de login mal sucedidas.
        /// </summary>
        public int FailedAttempts { get; private set; }

        /// <summary>
        /// Data e hora da última tentativa de login falha.
        /// </summary>
        public DateTime? LastFailedAt { get; private set; }

        /// <summary>
        /// Data e hora até a qual o usuário permanecerá bloqueado.
        /// </summary>
        public DateTime? LockedUntil { get; private set; }

        #endregion

        #region Constructors

        private LoginAttempts(
            int failedAttempts,
            DateTime? lastFailedAt,
            DateTime? lockedUntil)
        {
            FailedAttempts = failedAttempts;
            LastFailedAt = lastFailedAt;
            LockedUntil = lockedUntil;
        }

        private LoginAttempts() { }

        #endregion

        #region Factory

        /// <summary>
        /// Cria um novo controle de tentativas (sem falhas registradas).
        /// </summary>
        public static LoginAttempts Create()
        {
            return new LoginAttempts(0, null, null);
        }

        #endregion

        #region Behavior

        /// <summary>
        /// Registra uma nova tentativa de login falha.
        /// Bloqueia temporariamente o usuário se atingir o número máximo permitido.
        /// </summary>
        public LoginAttempts RegisterFailure()
        {
            var now = DateTime.UtcNow;
            var failed = FailedAttempts + 1;

            var lockedUntil = failed >= MAX_ATTEMPTS
                ? now.Add(LOCK_DURATION)
                : LockedUntil;

            return new LoginAttempts(failed, now, lockedUntil);
        }

        /// <summary>
        /// Reseta o contador de tentativas, liberando o usuário.
        /// </summary>
        public LoginAttempts Reset()
        {
            return new LoginAttempts(0, null, null);
        }

        /// <summary>
        /// Indica se o usuário está bloqueado por excesso de tentativas.
        /// </summary>
        public bool IsLocked()
        {
            return LockedUntil.HasValue && LockedUntil > DateTime.UtcNow;
        }

        /// <summary>
        /// Retorna uma mensagem de bloqueio com o tempo restante.
        /// </summary>
        public string? GetLockMessage()
        {
            if (!IsLocked())
                return null;

            var remaining = LockedUntil!.Value - DateTime.UtcNow;

            if (remaining.TotalSeconds < 60)
                return $"A conta está temporariamente bloqueada. Tente novamente em {Math.Ceiling(remaining.TotalSeconds)} segundos.";

            return $"A conta está temporariamente bloqueada. Tente novamente em {Math.Ceiling(remaining.TotalMinutes)} minutos.";
        }

        #endregion
    }
}
