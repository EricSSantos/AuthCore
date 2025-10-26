using AuthCore.Domain.Aggregates.EmailAggregate.Payloads;

namespace AuthCore.Domain.Aggregates.EmailAggregate.Strategies
{
    /// <summary>
    /// Representa o e-mail de recuperação de senha.
    /// Contém um código temporário de 6 dígitos para redefinição de senha.
    /// </summary>
    public sealed class ForgotPasswordEmail : Email
    {
        #region Constructors

        internal ForgotPasswordEmail(string to, string fullName, ForgotPasswordPayload payload)
            : base(to, fullName, EmailType.ForgotPassword, payload)
        { }

        #endregion

        #region Factory

        /// <summary>
        /// Cria uma nova instância de e-mail de recuperação de senha.
        /// </summary>
        internal static ForgotPasswordEmail Create(string to, string fullName)
        {
            return new ForgotPasswordEmail(to, fullName, new ForgotPasswordPayload());
        }

        #endregion
    }
}
