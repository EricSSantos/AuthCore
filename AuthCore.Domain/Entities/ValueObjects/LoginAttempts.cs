namespace AuthCore.Domain.Entities.ValueObjects
{
    public sealed class LoginAttempts : ValueObject
    {
        #region Constants

        private const int MAX_ATTEMPTS = 5;
        private static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(15);

        #endregion

        #region Properties

        public int FailedAttempts { get; }
        public DateTimeOffset? LastFailedAt { get; }
        public DateTimeOffset? LockedUntil { get; }

        #endregion

        #region Constructors

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

        #endregion

        #region Factory

        public static LoginAttempts Create()
        {
            return new LoginAttempts(0, null, null);
        }

        #endregion

        #region Behavior

        public LoginAttempts RegisterFailure()
        {
            var now = DateTimeOffset.UtcNow;
            var failedAttempts = FailedAttempts + 1;
            var lockedUntil = failedAttempts >= MAX_ATTEMPTS
                ? now.Add(LockDuration)
                : LockedUntil;

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

        protected override IEnumerable<object> GetValues()
        {
            yield return FailedAttempts;
            yield return LastFailedAt ?? DateTimeOffset.MinValue;
            yield return LockedUntil ?? DateTimeOffset.MinValue;
        }

        #endregion
    }
}
