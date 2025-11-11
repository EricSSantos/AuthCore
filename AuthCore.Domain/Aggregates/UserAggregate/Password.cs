using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Base;
using System.Text.RegularExpressions;

namespace AuthCore.Domain.Aggregates.UserAggregate
{
    /// <summary>
    /// Representa uma senha validada e segura do usuário.
    /// </summary>
    public sealed class Password : IValueObject
    {
        #region Constants

        private const int MIN_LENGTH = 8;
        private const int MAX_LENGTH = 24;

        #endregion

        public string Value { get; }

        private Password(string hashedPassword)
        {
            Value = hashedPassword;
        }

        private Password() { }

        /// <summary>
        /// Cria uma nova instância de Password com a senha já criptografada.
        /// </summary>
        public static Password Create(string hashedPassword)
        {
            return new Password(hashedPassword);
        }

        /// <summary>
        /// Valida os critérios mínimos de segurança da senha.
        /// </summary>
        /// <exception cref="BadRequestException"></exception>
        public static void Validate(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new BadRequestException("A senha não pode estar vazia.");

            if (password.Length < MIN_LENGTH || password.Length > MAX_LENGTH)
                throw new BadRequestException($"A senha deve conter entre {MIN_LENGTH} e {MAX_LENGTH} caracteres.");

            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,24}$");

            if (!regex.IsMatch(password))
                throw new BadRequestException("A senha deve conter letras maiúsculas, minúsculas, números e caracteres especiais.");
        }

        /// <summary>
        /// Valida a senha e confirma se ambos os valores correspondem.
        /// </summary>
        /// <exception cref="BadRequestException"></exception>
        public static void ValidateWithConfirmation(string password, string confirmPassword)
        {
            Validate(password);

            if (string.IsNullOrWhiteSpace(confirmPassword))
                throw new BadRequestException("A confirmação de senha não pode estar vazia.");

            if (password != confirmPassword)
                throw new BadRequestException("As senhas não correspondem.");
        }
    }
}
