using AuthCore.Domain.Enums;
using AuthCore.Domain.Exceptions;

namespace AuthCore.Domain.Entities
{
    public class User : Entity
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public Role Role { get; private set; }
        public bool Atictive { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? InactivatedAt { get; private set; }

        protected User() { }

        public User(string firstName, string lastName, string email, string password, Role role)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("O nome é obrigatório.");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("O sobrenome é obrigatório.");
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("O email é obrigatório.");
            if (string.IsNullOrWhiteSpace(password))
                throw new DomainException("O segredo ou senha são obrigatórios.");

            FirstName = firstName;
            LastName = lastName;
            Email = email.ToLowerInvariant();
            Password = password;
            Role = role;
            Atictive = false;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public void Activate()
        {
            if (Atictive)
                return;

            Atictive = true;
            InactivatedAt = null;
        }

        public void Inactivate()
        {
            if (!Atictive)
                return;

            Atictive = false;
            InactivatedAt = DateTimeOffset.UtcNow;
        }
    }
}