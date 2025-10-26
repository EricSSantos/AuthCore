using System.Security.Cryptography;

namespace AuthCore.Domain.Aggregates.EmailAggregate.Payloads
{
    /// <summary>
    /// Representa o conteúdo de um e-mail de recuperação de senha.
    /// Contém o código numérico de 6 dígitos que será validado pelo usuário.
    /// </summary>
    public sealed class ForgotPasswordPayload : EmailPayload
    {
        #region Constants

        private const int CODE_MIN = 100_000;
        private const int CODE_MAX = 1_000_000;

        #endregion

        #region Properties

        /// <summary>
        /// Código de verificação de 6 dígitos gerado de forma criptograficamente segura.
        /// </summary>
        public int Code { get; init; }

        #endregion

        #region Constructors

        public ForgotPasswordPayload()
        {
            Code = GenerateCode();
        }

        #endregion

        #region Helpers

        private static int GenerateCode()
        {
            return RandomNumberGenerator.GetInt32(CODE_MIN, CODE_MAX);
        }

        #endregion
    }
}
