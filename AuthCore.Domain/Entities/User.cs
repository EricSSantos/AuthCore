using AuthCore.Domain.Enums;
using AuthCore.Domain.Exceptions;
using AuthCore.Domain.ValueObjects;

namespace AuthCore.Domain.Entities
{
    public class User : Entity
    {
        #region Properties

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public Role Role { get; private set; }
        public bool Active { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? InactivatedAt { get; private set; }
        public LoginAttempts LoginAttempts { get; private set; }

        #endregion

        #region Constructors

        protected User() { }

        private User(
            string firstName,
            string lastName,
            string email,
            string password,
            Role role,
            bool active)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            Role = role;
            Active = active;
            CreatedAt = DateTimeOffset.UtcNow;
            LoginAttempts = LoginAttempts.Create();
        }

        #endregion

        #region Methods

        public static User Create(
            string firstName,
            string lastName,
            string email,
            string password,
            Role role = Role.Member,
            bool active = true)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("O nome é obrigatório.");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("O sobrenome é obrigatório.");
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("O email é obrigatório.");
            if (string.IsNullOrWhiteSpace(password))
                throw new DomainException("A senha é obrigatória.");

            return new User(firstName, lastName, email.ToLowerInvariant(), password, role, active);
        }

        public void Activate()
        {
            if (!Active)
            {
                Active = true;
                InactivatedAt = null;
            }
        }

        public void Inactivate()
        {
            if (Active)
            {
                Active = false;
                InactivatedAt = DateTimeOffset.UtcNow;
            }
        }

        #region Security

        public void RegisterFailedLogin()
        {
            LoginAttempts = LoginAttempts.FailedAttempt();
        }

        public void ResetLoginAttempts()
        {
            LoginAttempts = LoginAttempts.Reset();
        }

        public bool IsLocked()
        {
            return LoginAttempts.IsLocked();
        }

        public string? GetLockMessage()
        {
            return LoginAttempts.GetLockMessage();
        }

        #endregion

        #endregion
    }
}
