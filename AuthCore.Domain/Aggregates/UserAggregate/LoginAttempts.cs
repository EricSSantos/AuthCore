using AuthCore.Domain.Core.Interfaces.Base;

namespace AuthCore.Domain.Aggregates.UserAggregate
{
    /// <summary>
    /// Controla tentativas de login e bloqueios temporários.
    /// </summary>
    public sealed class LoginAttempts : IValueObject
    {
        #region Constants

        private const int MAX_ATTEMPTS = 5;
        private static readonly TimeSpan LOCK_DURATION = TimeSpan.FromMinutes(15);

        #endregion

        public int FailedAttempts { get; private set; }
        public DateTime? LastFailedAt { get; private set; }
        public DateTime? LockedUntil { get; private set; }

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

        /// <summary>
        /// Cria um novo controle de tentativas.
        /// </summary>
        public static LoginAttempts Create()
        {
            return new LoginAttempts(0, null, null);
        }

        /// <summary>
        /// Registra uma falha e aplica bloqueio se necessário.
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
        /// Reseta contadores e remove bloqueios.
        /// </summary>
        public LoginAttempts Reset()
        {
            return new LoginAttempts(0, null, null);
        }

        /// <summary>
        /// Verifica se o usuário está bloqueado.
        /// </summary>
        public bool IsLocked()
        {
            return LockedUntil.HasValue && LockedUntil > DateTime.UtcNow;
        }

        /// <summary>
        /// Obtém mensagem de bloqueio com o tempo restante.
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
    }
}
