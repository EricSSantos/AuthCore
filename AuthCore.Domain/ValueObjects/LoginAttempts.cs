namespace AuthCore.Domain.ValueObjects
{
    public sealed class LoginAttempts
    {
        public int FailedAttempts { get; private set; }
        public DateTimeOffset? LastFailedAt { get; private set; }
        public DateTimeOffset? LockedUntil { get; private set; }

        private const int MAX_ATTEMPTS = 5;
        private static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(15);

        protected LoginAttempts() { }

        private LoginAttempts(
            int failedAttempts,
            DateTimeOffset? lastFailedAt,
            DateTimeOffset? lockedUntil)
        {
            FailedAttempts = failedAttempts;
            LastFailedAt = lastFailedAt;
            LockedUntil = lockedUntil;
        }

        public static LoginAttempts Create()
        {
            return new LoginAttempts(0, null, null);
        }

        public LoginAttempts FailedAttempt()
        {
            var now = DateTimeOffset.UtcNow;
            var failedAttempts = FailedAttempts + 1;
            var lockedUntil = failedAttempts >= MAX_ATTEMPTS ? now.Add(LockDuration) : LockedUntil;

            return new LoginAttempts(failedAttempts, now, lockedUntil);
        }

        public LoginAttempts Reset()
        {
            return new LoginAttempts(0, null, null);
        }

        public bool IsLocked()
        {
            return LockedUntil.HasValue && LockedUntil > DateTimeOffset.UtcNow;
        }

        public string? GetLockMessage()
        {
            if (IsLocked())
                return $"Conta bloqueada até {LockedUntil:HH:mm:ss}.";

            return null;
        }
    }
}
