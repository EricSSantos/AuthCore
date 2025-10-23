using AuthCore.Domain.Commons.Exceptions;
using AuthCore.Domain.Shared;

namespace AuthCore.Domain.Aggregates.UserAggregate
{
    public sealed class User : Entity
    {
        #region Properties

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public Password Password { get; private set; }
        public RoleType Role { get; private set; }
        public bool Active { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? InactivatedAt { get; private set; }
        public LoginAttempts LoginAttempts { get; private set; }

        #endregion

        #region Constructors

        protected User() 
        { }

        private User(
            string firstName,
            string lastName,
            string email,
            string hashedPassword,
            RoleType role,
            bool active)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("O nome é obrigatório.");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("O sobrenome é obrigatório.");
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("O e-mail é obrigatório.");
            if (string.IsNullOrWhiteSpace(hashedPassword))
                throw new DomainException("A senha é obrigatória.");

            FirstName = firstName;
            LastName = lastName;
            Email = email.ToLowerInvariant();
            Password = Password.Create(hashedPassword);
            Role = role;
            Active = active;
            CreatedAt = DateTime.UtcNow;
            LoginAttempts = LoginAttempts.Create();
        }

        #endregion

        #region Factory

        public static User Create(
            string firstName,
            string lastName,
            string email,
            string password,
            RoleType role = RoleType.User,
            bool active = true)
        {
            return new User(firstName, lastName, email, password, role, active);
        }

        #endregion

        #region Behavior

        public string FullName
        {
            get { return $"{FirstName} {LastName}"; }
        }

        public bool IsActive()
        {
            if (LoginAttempts.IsLocked())
                return false;

            return Active;
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
                InactivatedAt = DateTime.UtcNow;
            }
        }

        public void SignIn(bool passwordIsValid)
        {
            if (LoginAttempts.IsLocked())
                throw new UnauthorizedException(LoginAttempts.GetLockMessage()!);

            if (!passwordIsValid)
            {
                LoginAttempts = LoginAttempts.RegisterFailure();
                throw new UnauthorizedException("E-mail ou senha inválidos.");
            }

            if (LoginAttempts.FailedAttempts > 0)
                LoginAttempts = LoginAttempts.Reset();
        }

        #endregion
    }
}
