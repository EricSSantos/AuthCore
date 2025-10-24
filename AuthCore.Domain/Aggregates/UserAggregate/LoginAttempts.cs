using AuthCore.Domain.Shared;

namespace AuthCore.Domain.Aggregates.UserAggregate
{
    public sealed class LoginAttempts : ValueObject
    {
        #region Constants

        private const int MAX_ATTEMPTS = 5;
        private static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(15);

        #endregion

        #region Properties

        public int FailedAttempts { get; }
        public DateTime? LastFailedAt { get; }
        public DateTime? LockedUntil { get; }

        #endregion

        #region Constructors

        protected LoginAttempts() { }

        private LoginAttempts(
            int failedAttempts,
            DateTime? lastFailedAt,
            DateTime? lockedUntil)
        {
            FailedAttempts = failedAttempts;
            LastFailedAt = lastFailedAt;
            LockedUntil = lockedUntil;
        }

        #endregion

        #region Factory

        public static LoginAttempts Create()
        {
            return new LoginAttempts(0, null, null);
        }

        #endregion

        #region Behavior

        /// <summary>
        /// Registra uma tentativa de login mal sucedida.
        /// Ao atingir o número máximo permitido, bloqueia temporariamente a conta.
        /// </summary>
        public LoginAttempts RegisterFailure()
        {
            var now = DateTime.UtcNow;
            var failedAttempts = FailedAttempts + 1;

            // Se atingiu o limite, bloqueia até X minutos no futuro
            var lockedUntil = failedAttempts >= MAX_ATTEMPTS
                ? now.Add(LockDuration)
                : LockedUntil;

            return new LoginAttempts(failedAttempts, now, lockedUntil);
        }

        /// <summary>
        /// Reseta as tentativas de login, liberando o usuário.
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
        /// Retorna a mensagem de bloqueio.
        /// </summary>
        public string? GetLockMessage()
        {
            if (IsLocked())
            {
                var remaining = LockedUntil!.Value - DateTime.UtcNow;

                // Se faltar menos de 60 segundos, exibe em segundos, senão, em minutos
                if (remaining.TotalSeconds < 60)
                    return $"A conta está temporariamente bloqueada. Tente novamente em {Math.Ceiling(remaining.TotalSeconds)} segundos.";

                return $"A conta está temporariamente bloqueada. Tente novamente em {Math.Ceiling(remaining.TotalMinutes)} minutos.";
            }

            return null;
        }

        protected override IEnumerable<object> GetValues()
        {
            yield return FailedAttempts;
            yield return LastFailedAt ?? DateTime.MinValue;
            yield return LockedUntil ?? DateTime.MinValue;
        }

        #endregion
    }
}
