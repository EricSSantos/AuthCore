using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Base;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;

namespace AuthCore.Domain.Aggregates.UserAggregate
{
    /// <summary>
    /// Representa um usuário autenticável.
    /// </summary>
    public sealed class User : IAggregateRoot
    {
        public Guid Id { get; private set; }
        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public string FullName { get { return $"{FirstName} {LastName}"; } }
        public Email Email { get; private set; } = null!;
        public Password Password { get; private set; } = null!;
        public Role Role { get; private set; }
        public bool Verified { get; private set; }
        public bool Active { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? InactivatedAt { get; private set; }
        public LoginAttempts LoginAttempts { get; private set; } = null!;

        #region Constructors

        private User() { }

        private User(
            Guid id,
            string firstName,
            string lastName,
            string email,
            string password,
            Role role,
            bool verified,
            bool active,
            DateTime createdAt,
            DateTime? updatedAt,
            DateTime? inactivatedAt,
            LoginAttempts loginAttempts)
        {
            Id = id;
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Email = Email.Create(email);
            Password = Password.Create(password);
            Role = role;
            Verified = verified;
            Active = active;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            InactivatedAt = inactivatedAt;
            LoginAttempts = loginAttempts;
            Validate();
        }

        #endregion

        #region Factory

        public static User Create(
            string firstName,
            string lastName,
            string email,
            string password,
            Role role = Role.User)
        {
            return new User(
                Guid.NewGuid(),
                firstName,
                lastName,
                email,
                password,
                role,
                verified: false,
                active: false,
                createdAt: DateTime.UtcNow,
                updatedAt: null,
                inactivatedAt: null,
                loginAttempts: LoginAttempts.Create()
            );
        }

        public static User Restore(
            Guid id,
            string firstName,
            string lastName,
            string email,
            string password,
            Role role,
            bool verified,
            bool active,
            DateTime createdAt,
            DateTime? updatedAt,
            DateTime? inactivatedAt,
            LoginAttempts loginAttempts)
        {
            return new User(
                id,
                firstName,
                lastName,
                email,
                password,
                role,
                verified,
                active,
                createdAt,
                updatedAt,
                inactivatedAt,
                loginAttempts
            );
        }

        #endregion

        public void Authenticate(string rawPassword, IPasswordHasher passwordHasher)
        {
            if (!Active || !Verified)
                throw new ForbiddenException("Usuário inativo ou não verificado.");

            if (LoginAttempts.IsLocked())
                throw new UnauthorizedException(LoginAttempts.GetLockMessage()!);

            if (!passwordHasher.IsValid(rawPassword, Password.Value))
            {
                LoginAttempts = LoginAttempts.RegisterFailure();
                throw new UnauthorizedException("E-mail ou senha inválidos.");
            }

            if (LoginAttempts.FailedAttempts > 0)
                LoginAttempts = LoginAttempts.Reset();
        }

        public void ChangePassword(string passwordHash)
        {
            Password = Password.Create(passwordHash);
            UpdatedAt = DateTime.UtcNow;
        }

        public void Confirm()
        {
            Verified = true;
            Active = true;
            UpdatedAt = DateTime.UtcNow;
        }

        private void Validate()
        {
            List<string> errors = new();

            if (Id == Guid.Empty)
                errors.Add("Id inválido.");

            if (string.IsNullOrWhiteSpace(FirstName))
                errors.Add("O primeiro nome é obrigatório.");

            if (string.IsNullOrWhiteSpace(LastName))
                errors.Add("O sobrenome é obrigatório.");

            if (Email is null)
                errors.Add("O e-mail é obrigatório.");

            if (Password is null)
                errors.Add("A senha é obrigatória.");

            if (!Enum.IsDefined(typeof(Role), Role))
                errors.Add("Perfil de usuário inválido.");

            if (CreatedAt == default)
                errors.Add("A data de criação é obrigatória.");

            if (UpdatedAt.HasValue && UpdatedAt.Value < CreatedAt)
                errors.Add("A data de atualização não pode ser anterior à criação.");

            if (InactivatedAt.HasValue && Active)
                errors.Add("Um usuário ativo não pode ter data de inativação.");

            if (LoginAttempts is null)
                errors.Add("Os dados de tentativas de login são obrigatórios.");

            if (errors.Count > 0)
                throw new BadRequestException(errors);
        }
    }
}
