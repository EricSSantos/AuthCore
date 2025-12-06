using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Base;

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
        public Password Password { get; private set; }
        public Role Role { get; private set; }
        public bool Verified { get; private set; } = false;
        public bool Active { get; private set; } = false;
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? InactivatedAt { get; private set; }
        public LoginAttempts LoginAttempts { get; private set; }

        #region Constructors

        private User() { }

        private User(
            Guid id,
            string firstName,
            string lastName,
            Email email,
            string passwordHash,
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
            Email = email;
            Password = Password.Create(passwordHash);
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

        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        /// <param name="firstName">Nome do usuário.</param>
        /// <param name="lastName">Sobrenome do usuário.</param>
        /// <param name="email">E-mail do usuário.</param>
        /// <param name="passwordHash">Senha criptografada.</param>
        /// <param name="role">Perfil do usuário.</param>
        /// <returns>Instância criada de <see cref="User"/>.</returns>
        public static User Create(
            string firstName,
            string lastName,
            string email,
            string passwordHash,
            Role role = Role.User)
        {
            return new User(
                Guid.NewGuid(),
                firstName,
                lastName,
                Email.Create(email),
                passwordHash,
                role,
                verified: false,
                active: false,
                createdAt: DateTime.UtcNow,
                updatedAt: null,
                inactivatedAt: null,
                loginAttempts: LoginAttempts.Create()
            );
        }

        /// <summary>
        /// Restaura um usuário persistido.
        /// </summary>
        /// <returns>Instância restaurada de <see cref="User"/>.</returns>
        public static User Restore(
            Guid id,
            string firstName,
            string lastName,
            string email,
            string passwordHash,
            Role role,
            bool verified,
            bool active,
            DateTime createdAt,
            DateTime updatedAt,
            DateTime? inactivatedAt,
            LoginAttempts loginAttempts)
        {
            return new User(
                id,
                firstName,
                lastName,
                Email.Create(email),
                passwordHash,
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

        #region Behavior

        /// <summary>
        /// Altera a senha do usuário.
        /// </summary>
        /// <param name="passwordHash">Nova senha criptografada.</param>
        public void ChangePassword(string passwordHash)
        {
            Password = Password.Create(passwordHash);
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Confirma e ativa o usuário.
        /// </summary>
        public void Confirm()
        {
            Verified = true;
            Active = true;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Verifica se o usuário está apto à autenticação.
        /// </summary>
        /// <returns>True quando ativo, verificado e não bloqueado.</returns>
        public bool IsActive()
        {
            if (LoginAttempts.IsLocked())
                throw new UnauthorizedException(LoginAttempts.GetLockMessage()!);

            if (!Active || !Verified)
                return false;

            return true;
        }

        #endregion

        #region Validation

        /// <summary>
        /// Valida os dados essenciais do usuário.
        /// </summary>
        private void Validate()
        {
            if (Id == Guid.Empty)
                throw new BadRequestException("Identificador inválido.");

            if (string.IsNullOrWhiteSpace(FirstName))
                throw new BadRequestException("O nome é obrigatório.");

            if (string.IsNullOrWhiteSpace(LastName))
                throw new BadRequestException("O sobrenome é obrigatório.");

            if (Email is null)
                throw new BadRequestException("O e-mail é obrigatório.");

            if (Password is null)
                throw new BadRequestException("A senha é obrigatória.");

            if (!Enum.IsDefined(typeof(Role), Role))
                throw new BadRequestException("Perfil de usuário inválido.");

            if (CreatedAt == default)
                throw new BadRequestException("A data de criação é obrigatória.");

            if (UpdatedAt.HasValue && UpdatedAt.Value < CreatedAt)
                throw new BadRequestException("A data de atualização não pode ser anterior à criação.");

            if (InactivatedAt.HasValue && Active)
                throw new BadRequestException("Usuário ativo não pode ter data de inativação.");

            if (LoginAttempts is null)
                throw new BadRequestException("Os dados de tentativas de login são obrigatórios.");
        }

        #endregion
    }
}
