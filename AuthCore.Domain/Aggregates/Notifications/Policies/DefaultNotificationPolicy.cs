using AuthCore.Domain.Aggregates.Notifications.Payloads;
using AuthCore.Domain.Core.Exceptions;

namespace AuthCore.Domain.Aggregates.Notifications.Policies
{
    /// <summary>Representa a política padrão de notificação.</summary>
    public sealed class DefaultNotificationPolicy : INotificationPolicy
    {
        /// <summary>Operação para criar payload de notificação.</summary>
        /// <param name="type">Tipo da notificação.</param>
        /// <param name="code">Código da notificação.</param>
        public Payload CreatePayload(NotificationType type, int? code)
        {
            switch (type)
            {
                case NotificationType.ConfirmEmail:
                    if (!code.HasValue)
                        throw new BadRequestException("O código de confirmação é obrigatório.");
                    return new ConfirmEmailPayload(code.Value);
                case NotificationType.Welcome:
                    return new WelcomeEmailPayload();
                case NotificationType.ForgotPassword:
                    if (!code.HasValue)
                        throw new BadRequestException("O código de recuperação é obrigatório.");
                    return new ForgotPasswordPayload(code.Value);
                default:
                    throw new BadRequestException("O tipo de notificação informado é inválido.");
            }
        }
    }
}
