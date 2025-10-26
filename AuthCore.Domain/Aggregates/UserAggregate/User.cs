using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Base;
using System.Text.RegularExpressions;

namespace AuthCore.Domain.Aggregates.UserAggregate
{
    /// <summary>
    /// Representa um usuário autenticável dentro do sistema.
    /// </summary>
    public sealed class User : IAggregateRoot
    {
        #region Properties

        /// <summary>
        /// Identificador único do usuário.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Primeiro nome do usuário.
        /// </summary>
        public string FirstName { get; private set; }

        /// <summary>
        /// Sobrenome do usuário.
        /// </summary>
        public string LastName { get; private set; }

        /// <summary>
        /// Endereço de e-mail do usuário.
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// Senha criptografada do usuário.
        /// </summary>
        public Password Password { get; private set; }

        /// <summary>
        /// Papel ou função atribuída ao usuário (ex: Admin, User, etc.).
        /// </summary>
        public RoleType Role { get; private set; }

        /// <summary>
        /// Indica se o usuário está ativo no sistema.
        /// </summary>
        public bool Active { get; private set; }

        /// <summary>
        /// Data e hora de criação do usuário (UTC).
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Data em que o usuário foi inativado, se aplicável.
        /// </summary>
        public DateTime? InactivatedAt { get; private set; }

        /// <summary>
        /// Tentativas de login consecutivas com falha.
        /// </summary>
        public LoginAttempts LoginAttempts { get; private set; }

        #endregion

        #region Constructors

        private User() { }

        private User(
            Guid id,
            string firstName,
            string lastName,
            string email,
            string passwordHash,
            RoleType role,
            bool active)
        {
            Id = id;
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Email = email.Trim().ToLowerInvariant();
            Password = Password.Create(passwordHash);
            Role = role;
            Active = active;
            CreatedAt = DateTime.UtcNow;
            LoginAttempts = LoginAttempts.Create();
            Validate();
        }

        #endregion

        #region Factory

        /// <summary>
        /// Cria um novo usuário com dados validados.
        /// </summary>
        /// <param name="firstName">Primeiro nome do usuário.</param>
        /// <param name="lastName">Sobrenome do usuário.</param>
        /// <param name="email">E-mail do usuário.</param>
        /// <param name="passwordHash">Senha criptografada (hash).</param>
        /// <param name="role">Função atribuída (padrão: User).</param>
        /// <param name="active">Define se o usuário começa ativo (padrão: true).</param>
        /// <returns>Uma nova instância de <see cref="User"/> pronta para persistência.</returns>
        public static User Create(
            string firstName,
            string lastName,
            string email,
            string passwordHash,
            RoleType role = RoleType.User,
            bool active = true)
        {
            return new User(Guid.NewGuid(), firstName, lastName, email, passwordHash, role, active);
        }

        #endregion

        #region Behavior

        /// <summary>
        /// Retorna o nome completo do usuário.
        /// </summary>
        public string FullName
        {
            get { return $"{FirstName} {LastName}"; }
        }

        /// <summary>
        /// Verifica se o usuário está ativo e sem restrição por tentativas de login.
        /// </summary>
        /// <returns>Verdadeiro se o usuário estiver ativo; caso contrário, lança exceção.</returns>
        /// <exception cref="UnauthorizedException">Lançada quando o usuário está bloqueado por tentativas falhas.</exception>
        public bool IsActive()
        {
            if (LoginAttempts.IsLocked())
                throw new UnauthorizedException(LoginAttempts.GetLockMessage()!);

            return Active;
        }

        /// <summary>
        /// Altera a senha do usuário após validar o novo valor.
        /// </summary>
        /// <param name="passwordHash">Senha criptografada (hash).</param>
        public void ChangePassword(string passwordHash)
        {
            Password = Password.Create(passwordHash);
        }

        #endregion

        #region Validation

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(FirstName))
                throw new BadRequestException("O nome é obrigatório.");

            if (string.IsNullOrWhiteSpace(LastName))
                throw new BadRequestException("O sobrenome é obrigatório.");

            if (string.IsNullOrWhiteSpace(Email))
                throw new BadRequestException("O e-mail é obrigatório.");

            if (!IsValidEmail(Email))
                throw new BadRequestException("O e-mail informado é inválido.");

            if (Password is null)
                throw new BadRequestException("A senha é obrigatória.");
        }

        private static bool IsValidEmail(string email)
        {
            var regex = new Regex(
                @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase
            );

            return regex.IsMatch(email);
        }

        #endregion
    }
}
