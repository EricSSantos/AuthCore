using AuthCore.Domain.Exceptions;

namespace AuthCore.Domain.Entities
{
    public sealed class Session : Entity
    {
        public Guid UserId { get; private set; }
        public User User { get; private set; }
        public string RefreshToken { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset ExpiresAt { get; private set; }
        public DateTimeOffset? RevokedAt { get; private set; }

        protected Session() { }

        public Session(Guid userId, string refreshToken)
        {
            if (userId == Guid.Empty)
                throw new DomainException("O UserId é obrigatório.");
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new DomainException("O refresh_token é obrigatório.");

            UserId = userId;
            RefreshToken = refreshToken;
            CreatedAt = DateTimeOffset.UtcNow;
            ExpiresAt = CreatedAt.Add(TimeSpan.FromDays(7));
        }
    }
}
