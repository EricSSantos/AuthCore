using System.ComponentModel;

namespace AuthCore.Domain.Aggregates.ConfirmCodeAggregate
{
    /// <summary>
    /// Define os tipos de códigos de confirmação.
    /// </summary>
    public enum CodeType
    {
        /// <summary>
        /// Indica solicitação para confirmar e-mail.
        /// </summary>
        ConfirmEmail,

        /// <summary>
        /// Indica solicitação para redefinir senha.
        /// </summary>
        ForgotPassword
    }
}
