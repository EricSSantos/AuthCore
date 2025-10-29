using System.ComponentModel;

namespace AuthCore.Domain.Aggregates.ConfirmCodeAggregate
{
    /// <summary>
    /// Representa os tipos de códigos de confirmação da aplicação.
    /// </summary>
    public enum CodeType
    {
        [Description("Verificação de e-mail.")]
        ConfirmEmail,
        [Description("Recuperação de senha.")]
        ForgotPassword
    }
}
