using AuthCore.Domain.Aggregates.Notifications.Payloads;

namespace AuthCore.Domain.Aggregates.Notifications.Policies
{
    /// <summary>Define operações de política de notificação.</summary>
    public interface INotificationPolicy
    {
        /// <summary>Operação para criar payload de notificação.</summary>
        /// <param name="type">Tipo da notificação.</param>
        /// <param name="code">Código da notificação.</param>
        Payload CreatePayload(NotificationType type, int? code);
    }
}
