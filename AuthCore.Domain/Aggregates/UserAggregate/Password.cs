using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Base;
using System.Text.RegularExpressions;

namespace AuthCore.Domain.Aggregates.UserAggregate
{
    /// <summary>
    /// Representa uma senha segura e validada do usuário.
    /// </summary>
    public sealed class Password : IValueObject
    {
        #region Constants

        private const int MIN_LENGTH = 8;
        private const int MAX_LENGTH = 24;

        #endregion

        #region Properties

        /// <summary>
        /// Valor da senha criptografada.
        /// </summary>
        public string Value { get; }

        #endregion

        #region Constructors

        private Password(string hashedPassword)
        {
            Value = hashedPassword;
        }

        private Password() { }

        #endregion

        #region Factory

        /// <summary>
        /// Cria uma nova instância de senha a partir de um valor já criptografado.
        /// </summary>
        /// <param name="hashedPassword">Senha já criptografada (hash).</param>
        /// <returns>Um novo objeto <see cref="Password"/> com o valor informado.</returns>
        public static Password Create(string hashedPassword)
        {
            return new Password(hashedPassword);
        }

        #endregion

        #region Validation

        /// <summary>
        /// Valida se a senha atende aos requisitos mínimos de segurança.
        /// </summary>
        /// <param name="password">Senha em texto puro.</param>
        /// <exception cref="BadRequestException">
        /// Lançada quando a senha está vazia, fora do tamanho permitido ou não atende aos critérios de complexidade.
        /// </exception>
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
        /// Valida a senha e verifica se a confirmação informada correspondem e atende aos requisitos.
        /// </summary>
        /// <param name="password">Senha em texto puro.</param>
        /// <param name="confirmPassword">Confirmação da senha.</param>
        /// <exception cref="BadRequestException">
        /// Lançada quando a confirmação está vazia ou as senhas não correspondem.
        /// </exception>
        public static void ValidateWithConfirmation(string password, string confirmPassword)
        {
            Validate(password);

            if (string.IsNullOrWhiteSpace(confirmPassword))
                throw new BadRequestException("A confirmação de senha não pode estar vazia.");

            if (password != confirmPassword)
                throw new BadRequestException("As senhas não correspondem.");
        }

        #endregion
    }
}
