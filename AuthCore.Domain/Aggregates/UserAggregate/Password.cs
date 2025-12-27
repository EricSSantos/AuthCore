using System;
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
        /// Cria a senha com o valor já criptografado.
        /// </summary>
        /// <param name="hashedPassword">Senha criptografada.</param>
        /// <returns>Instância criada de <see cref="Password"/>.</returns>
        public static Password Create(string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword))
                throw new BadRequestException("A senha criptografada não pode estar vazia.");

            return new Password(hashedPassword);
        }

        /// <summary>
        /// Valida os critérios mínimos da senha.
        /// </summary>
        /// <param name="password">Senha a validar.</param>
        /// <exception cref="BadRequestException">Lançada quando o formato é inválido.</exception>
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
        /// Valida a senha e confirma se ambas correspondem.
        /// </summary>
        /// <param name="password">Senha informada.</param>
        /// <param name="confirmPassword">Confirmação da senha.</param>
        /// <exception cref="BadRequestException">Lançada quando os valores não são compatíveis.</exception>
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
