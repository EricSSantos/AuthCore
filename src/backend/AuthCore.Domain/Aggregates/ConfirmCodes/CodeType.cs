using System.ComponentModel;

namespace AuthCore.Domain.Aggregates.ConfirmCodes
{
    /// <summary>Define os tipos de códigos de confirmação.</summary>
    public enum CodeType
    {
        /// <summary>Código de confirmação de e-mail.</summary>
        [Description("Código de confirmação de e-mail.")]
        ConfirmEmail = 1,

        /// <summary>Código de recuperação de senha.</summary>
        [Description("Código de recuperação de senha.")]
        ForgotPassword = 2
    }
}
