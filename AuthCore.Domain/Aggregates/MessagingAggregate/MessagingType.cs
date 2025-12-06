namespace AuthCore.Domain.Aggregates.MessagingAggregate
{
    /// <summary>
    /// Define os tipos de e-mails enviados pela aplicação.
    /// </summary>
    public enum MessagingType
    {
        /// <summary>
        /// Indica e-mail para confirmação de conta.
        /// </summary>
        ConfirmEmail,

        /// <summary>
        /// Indica e-mail de boas-vindas.
        /// </summary>
        Welcome,

        /// <summary>
        /// Indica e-mail para recuperação de senha.
        /// </summary>
        ForgotPassword
    }
}
