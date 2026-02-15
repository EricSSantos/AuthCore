using AuthCore.Domain.Aggregates.Notifications.Payloads;
using AuthCore.Domain.Aggregates.Users;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Base;

namespace AuthCore.Domain.Aggregates.Notifications
{
    /// <summary>Representa uma notificação gerada pela aplicação.</summary>
    public sealed class Notification : IAggregateRoot
    {
        public Guid Id { get; private set; }
        public string To { get; private set; } = null!;
        public string FullName { get; private set; } = null!;
        public NotificationType Type { get; private set; }
        public Payload? Payload { get; private set; }
        public DateTime CreatedAt { get; private set; }

        #region Constructors

        /// <summary>Operação para criar instância de notificação.</summary>
        /// <param name="id">Identificador da notificação.</param>
        /// <param name="to">E-mail do destinatário.</param>
        /// <param name="fullName">Nome completo do destinatário.</param>
        /// <param name="type">Tipo da notificação.</param>
        /// <param name="createdAt">Data de criação da notificação.</param>
        /// <param name="payload">Conteúdo da notificação.</param>
        private Notification(
            Guid id,
            string to,
            string fullName,
            NotificationType type,
            DateTime createdAt,
            Payload? payload = null)
        {
            Id = id;
            To = to.Trim();
            FullName = fullName.Trim();
            Type = type;
            Payload = payload;
            CreatedAt = createdAt;
            Validate();
        }

        #endregion

        #region Factory

        /// <summary>Operação para criar notificação.</summary>
        /// <param name="to">E-mail do destinatário.</param>
        /// <param name="fullName">Nome completo do destinatário.</param>
        /// <param name="type">Tipo da notificação.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        /// <param name="payload">Conteúdo da notificação.</param>
        public static Notification Create(
            string to, 
            string fullName, 
            NotificationType type,
            DateTime utcNow,
            Payload payload)
        {
            return new Notification(
                Guid.NewGuid(), 
                to, fullName, 
                type,
                utcNow,
                payload
            );
        }

        #endregion

        /// <summary>Operação para obter payload da notificação.</summary>
        public TPayload GetPayload<TPayload>() where TPayload : Payload
        {
            if (Payload is not TPayload typedPayload)
                throw new BadRequestException(
                    $"O payload do tipo '{typeof(TPayload).Name}' não é compatível com a notificação '{Type}'.");

            return typedPayload;
        }

        #region Validation

        /// <summary>Operação para validar notificação.</summary>
        private void Validate()
        {
            List<string> errors = new();

            Email.Validate(To);

            if (string.IsNullOrWhiteSpace(FullName))
                errors.Add("O nome do destinatário é obrigatório.");

            if (!Enum.IsDefined(typeof(NotificationType), Type))
                errors.Add("O tipo de notificação é inválido.");

            if (Payload is null)
                errors.Add("O conteúdo é obrigatório.");
            else if (!IsValidPayload(Type, Payload))
                errors.Add("O conteúdo da notificação não é compatível com o tipo informado.");

            if (errors.Count > 0)
                throw new BadRequestException(errors);
        }

        /// <summary>Operação para validar compatibilidade do payload.</summary>
        /// <param name="type">Tipo da notificação.</param>
        /// <param name="payload">Conteúdo da notificação.</param>
        private static bool IsValidPayload(NotificationType type, Payload payload)
        {
            switch (type)
            {
                case NotificationType.ConfirmEmail:
                    return payload is ConfirmEmailPayload;
                case NotificationType.Welcome:
                    return payload is WelcomeEmailPayload;
                case NotificationType.ForgotPassword:
                    return payload is ForgotPasswordPayload;
                default:
                    return false;
            }
        }

        #endregion
    }
}
