namespace AuthCore.Domain.Aggregates.Notifications.Payloads
{
    /// <summary>Representa a base para payloads de e-mail.</summary>
    public abstract class Payload
    {
        /// <summary>Tipo de notificação compatível com o payload.</summary>
        public abstract NotificationType Type { get; }
    }
}
