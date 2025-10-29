using System.ComponentModel;

namespace AuthCore.Domain.Aggregates.EmailAggregate
{
    /// <summary>
    /// Define os tipos de e-mails que podem ser enviados pela aplicação.
    /// </summary>
    public enum EmailType
    {
        [Description("E-mail de confirmação de conta")]
        ConfirmEmail,
        [Description("E-mail de boas-vindas")]
        Welcome,
        [Description("E-mail de recuperação de senha")]
        ForgotPassword
    }
}
