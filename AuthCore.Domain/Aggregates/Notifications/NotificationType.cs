namespace AuthCore.Domain.Aggregates.Notifications
{
    /// <summary>Define os tipos de e-mails enviados pela aplicação.</summary>
    public enum NotificationType
    {
        ConfirmEmail,
        Welcome,
        ForgotPassword
    }
}
