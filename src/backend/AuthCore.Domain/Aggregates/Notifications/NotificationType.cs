using System.ComponentModel;

namespace AuthCore.Domain.Aggregates.Notifications
{
    /// <summary>Define os tipos de e-mails enviados pela aplicação.</summary>
    public enum NotificationType
    {
        /// <summary>E-mail de confirmação de conta.</summary>
        [Description("E-mail de confirmação de conta.")]
        ConfirmEmail = 1,

        /// <summary>E-mail de boas-vindas.</summary>
        [Description("E-mail de boas-vindas.")]
        Welcome = 2,

        /// <summary>E-mail de recuperação de senha.</summary>
        [Description("E-mail de recuperação de senha.")]
        ForgotPassword = 3
    }
}
