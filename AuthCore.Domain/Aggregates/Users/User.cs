using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Base;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;
using AuthCore.Domain.Aggregates.Users.Interfaces;

namespace AuthCore.Domain.Aggregates.Users
{
    /// <summary>Representa um usuário autenticável.</summary>
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

        /// <summary>Operação para criar instância para EF Core.</summary>
        private User() { }

        /// <summary>Operação para criar instância de usuário.</summary>
        /// <param name="id">Identificador do usuário.</param>
        /// <param name="firstName">Primeiro nome do usuário.</param>
        /// <param name="lastName">Sobrenome do usuário.</param>
        /// <param name="email">E-mail do usuário.</param>
        /// <param name="password">Senha criptografada do usuário.</param>
        /// <param name="role">Perfil do usuário.</param>
        /// <param name="verified">Indicador de confirmação do usuário.</param>
        /// <param name="active">Indicador de ativação do usuário.</param>
        /// <param name="createdAt">Data de criação do usuário.</param>
        /// <param name="updatedAt">Data de atualização do usuário.</param>
        /// <param name="inactivatedAt">Data de inativação do usuário.</param>
        /// <param name="loginAttempts">Tentativas de login do usuário.</param>
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

        /// <summary>Operação para criar usuário.</summary>
        /// <param name="firstName">Primeiro nome do usuário.</param>
        /// <param name="lastName">Sobrenome do usuário.</param>
        /// <param name="email">E-mail do usuário.</param>
        /// <param name="password">Senha criptografada do usuário.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        /// <param name="role">Perfil do usuário.</param>
        public static User Create(
            string firstName,
            string lastName,
            string email,
            string password,
            DateTime utcNow,
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
                createdAt: utcNow,
                updatedAt: null,
                inactivatedAt: null,
                loginAttempts: LoginAttempts.Create()
            );
        }

        /// <summary>Operação para restaurar usuário.</summary>
        /// <param name="id">Identificador do usuário.</param>
        /// <param name="firstName">Primeiro nome do usuário.</param>
        /// <param name="lastName">Sobrenome do usuário.</param>
        /// <param name="email">E-mail do usuário.</param>
        /// <param name="password">Senha criptografada do usuário.</param>
        /// <param name="role">Perfil do usuário.</param>
        /// <param name="verified">Indicador de confirmação do usuário.</param>
        /// <param name="active">Indicador de ativação do usuário.</param>
        /// <param name="createdAt">Data de criação do usuário.</param>
        /// <param name="updatedAt">Data de atualização do usuário.</param>
        /// <param name="inactivatedAt">Data de inativação do usuário.</param>
        /// <param name="loginAttempts">Tentativas de login do usuário.</param>
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

        /// <summary>Operação para verificar ativação e confirmação.</summary>
        public bool IsActiveAndVerified()
        {
            return Active && Verified;
        }

        /// <summary>Operação para autenticar usuário.</summary>
        /// <param name="rawPassword">Senha informada do usuário.</param>
        /// <param name="passwordHasher">Serviço de validação de senha.</param>
        /// <param name="authenticationPolicy">Política de autenticação do usuário.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        public void Authenticate(
            string rawPassword,
            IPasswordHasher passwordHasher,
            IUserAuthenticationPolicy authenticationPolicy,
            DateTime utcNow)
        {
            authenticationPolicy.EnsureCanAuthenticate(this, utcNow);

            if (!passwordHasher.IsValid(rawPassword, Password.Value))
            {
                LoginAttempts = authenticationPolicy.RegisterFailure(LoginAttempts, utcNow);
                UpdatedAt = utcNow;
                throw new UnauthorizedException("E-mail ou senha inválidos.");
            }

            if (LoginAttempts.FailedAttempts > 0)
            {
                LoginAttempts = LoginAttempts.Reset();
                UpdatedAt = utcNow;
            }
        }

        /// <summary>Operação para alterar senha do usuário.</summary>
        /// <param name="passwordHash">Senha criptografada do usuário.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        public void ChangePassword(string passwordHash, DateTime utcNow)
        {
            Password = Password.Create(passwordHash);
            UpdatedAt = utcNow;
        }

        /// <summary>Operação para confirmar usuário.</summary>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        public void Confirm(DateTime utcNow)
        {
            if (Verified && Active)
                return;

            Verified = true;
            Active = true;
            InactivatedAt = null;
            UpdatedAt = utcNow;
        }

        /// <summary>Operação para ativar usuário.</summary>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        public void Activate(DateTime utcNow)
        {
            if (Active)
                return;

            Active = true;
            InactivatedAt = null;
            UpdatedAt = utcNow;
        }

        /// <summary>Operação para desativar usuário.</summary>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        public void Deactivate(DateTime utcNow)
        {
            if (!Active)
                return;

            Active = false;
            InactivatedAt = utcNow;
            UpdatedAt = utcNow;
        }

        #region Validation

        /// <summary>Operação para validar usuário.</summary>
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

        #endregion
    }
}
