using AuthCore.Domain.Core.Interfaces.Base;

namespace AuthCore.Domain.Aggregates.Users
{
    /// <summary>Representa controle de tentativas de login.</summary>
    public sealed class LoginAttempts : IValueObject
    {
        private const int MIN_ATTEMPTS = 1;

        public int FailedAttempts { get; private set; }
        public DateTime? LastFailedAt { get; private set; }
        public DateTime? LockedUntil { get; private set; }

        /// <summary>Operação para criar instância de tentativas.</summary>
        private LoginAttempts() { }

        /// <summary>Operação para criar instância de tentativas.</summary>
        /// <param name="failedAttempts">Quantidade de tentativas falhas.</param>
        /// <param name="lastFailedAt">Data da última falha.</param>
        /// <param name="lockedUntil">Data de expiração do bloqueio.</param>
        private LoginAttempts(
            int failedAttempts,
            DateTime? lastFailedAt,
            DateTime? lockedUntil)
        {
            FailedAttempts = failedAttempts;
            LastFailedAt = lastFailedAt;
            LockedUntil = lockedUntil;
        }

        /// <summary>Operação para criar controle de tentativas.</summary>
        public static LoginAttempts Create()
        {
            return new LoginAttempts(0, null, null);
        }

        /// <summary>Operação para restaurar controle de tentativas.</summary>
        /// <param name="failedAttempts">Quantidade de tentativas falhas.</param>
        /// <param name="lastFailedAt">Data da última falha.</param>
        /// <param name="lockedUntil">Data de expiração do bloqueio.</param>
        public static LoginAttempts Restore(
            int failedAttempts,
            DateTime? lastFailedAt,
            DateTime? lockedUntil)
        {
            return new LoginAttempts(failedAttempts, lastFailedAt, lockedUntil);
        }

        /// <summary>Operação para registrar falha de autenticação.</summary>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        /// <param name="maxAttempts">Quantidade máxima de tentativas.</param>
        /// <param name="lockDuration">Duração do bloqueio.</param>
        public LoginAttempts RegisterFailure(DateTime utcNow, int maxAttempts, TimeSpan lockDuration)
        {
            if (maxAttempts < MIN_ATTEMPTS)
                throw new ArgumentOutOfRangeException(nameof(maxAttempts));

            if (lockDuration <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(lockDuration));

            var failed = FailedAttempts + 1;

            var lockedUntil = failed >= maxAttempts
                ? utcNow.Add(lockDuration)
                : LockedUntil;

            return new LoginAttempts(failed, utcNow, lockedUntil);
        }

        /// <summary>Operação para resetar tentativas.</summary>
        public LoginAttempts Reset()
        {
            return new LoginAttempts(0, null, null);
        }

        /// <summary>Operação para verificar bloqueio.</summary>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        public bool IsLocked(DateTime utcNow)
        {
            return LockedUntil.HasValue && LockedUntil > utcNow;
        }

        /// <summary>Operação para obter mensagem de bloqueio.</summary>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        public string? GetLockMessage(DateTime utcNow)
        {
            if (!IsLocked(utcNow))
                return null;

            var remaining = LockedUntil!.Value - utcNow;

            if (remaining.TotalSeconds < 60)
                return $"A conta está temporariamente bloqueada. Tente novamente em {Math.Ceiling(remaining.TotalSeconds)} segundos.";

            return $"A conta está temporariamente bloqueada. Tente novamente em {Math.Ceiling(remaining.TotalMinutes)} minutos.";
        }
    }
}
