namespace AuthCore.Domain.Aggregates.MessagingAggregate
{
    /// <summary>
    /// Define os tipos de e-mails que podem ser enviados pela aplicação.
    /// </summary>
    public enum MessagingType
    {
        ConfirmEmail,
        Welcome,
        ForgotPassword
    }
}
