using AuthCore.Domain.Aggregates.EmailAggregate;
using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Domain.Commons.Exceptions;
using AuthCore.Domain.Shared;
using System.Text.RegularExpressions;

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

        protected User() { }

        private User(
            string firstName,
            string lastName,
            string email,
            string hashedPassword,
            RoleType role,
            bool active)
        {
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Email = email.Trim().ToLowerInvariant();
            Password = Password.Create(hashedPassword);
            Role = role;
            Active = active;
            CreatedAt = DateTime.UtcNow;
            LoginAttempts = LoginAttempts.Create();
            Validate();
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

        /// <summary>
        /// Retorna o nome completo do usuário.
        /// </summary>
        public string FullName
        {
            get { return $"{FirstName} {LastName}"; }
        }

        /// <summary>
        /// Indica se o usuário está ativo.
        /// </summary>
        public bool IsActive()
        {
            if (LoginAttempts.IsLocked())
            {
                return false;
            }

            return Active;
        }

        /// <summary>
        /// Realiza o processo de autenticação do usuário.
        /// </summary>
        /// <param name="passwordIsValid">Indica se a senha informada é válida.</param>
        /// <exception cref="UnauthorizedException">
        /// Lançada quando o usuário está bloqueado ou a senha é inválida.
        /// </exception>
        public void SignIn(bool passwordIsValid)
        {
            if (LoginAttempts.IsLocked())
            {
                throw new UnauthorizedException(LoginAttempts.GetLockMessage()!);
            }

            if (!passwordIsValid)
            {
                LoginAttempts = LoginAttempts.RegisterFailure();
                throw new UnauthorizedException("E-mail ou senha inválidos.");
            }

            if (LoginAttempts.FailedAttempts > 0)
            {
                LoginAttempts = LoginAttempts.Reset();
            }
        }

        /// <summary>
        /// Altera a senha do usuário.
        /// </summary>
        /// <param name="hashedPassword">Nova senha já criptografada.</param>
        /// <param name="password">Senha em texto puro para validação.</param>
        /// <param name="confirmPassword">Confirmação da nova senha.</param>
        public void ChangePassword(string hashedPassword, string password, string? confirmPassword = "")
        {
            Password.Validate(password, confirmPassword);
            Password = Password.Create(hashedPassword);
        }

        #endregion

        #region Private Methods

        private void Validate()
        {
            var validate = Validator();
            
            if (string.IsNullOrWhiteSpace(FirstName))
                validate.AddError("O nome é obrigatório.");
            if (string.IsNullOrWhiteSpace(LastName))
                validate.AddError("O sobrenome é obrigatório.");
            if (string.IsNullOrWhiteSpace(Email))
                validate.AddError("O e-mail é obrigatório.");
            else if (!IsValidEmail(Email))
                validate.AddError("O e-mail informado é inválido.");
            
            validate.ThrowIfInvalid();
        }

        private bool IsValidEmail(string email)
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
