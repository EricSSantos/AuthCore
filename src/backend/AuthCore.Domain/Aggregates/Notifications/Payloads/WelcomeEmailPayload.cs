namespace AuthCore.Domain.Aggregates.Notifications.Payloads
{
    /// <summary>Representa dados do e-mail de boas-vindas.</summary>
    public sealed class WelcomeEmailPayload : Payload
    {
        public override NotificationType Type
        {
            get { return NotificationType.Welcome; }
        }
    }
}
