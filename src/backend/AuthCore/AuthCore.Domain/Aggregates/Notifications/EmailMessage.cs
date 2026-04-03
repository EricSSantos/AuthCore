using AuthCore.Domain.Aggregates.Notifications.Payloads;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.ValueObjects;

namespace AuthCore.Domain.Aggregates.Notifications
{
    /// <summary>Representa uma mensagem de e-mail para despacho.</summary>
    public sealed class EmailMessage
    {
        public string To { get; }
        public string FullName { get; }
        public NotificationType Type { get; }
        public Payload Payload { get; }
        public DateTime CreatedAt { get; }

        /// <summary>Operação para criar instância de mensagem de e-mail.</summary>
        /// <param name="to">Destinatário do e-mail.</param>
        /// <param name="fullName">Nome completo do destinatário.</param>
        /// <param name="type">Tipo da notificação.</param>
        /// <param name="createdAt">Data de criação da mensagem.</param>
        /// <param name="payload">Conteúdo da notificação.</param>
        private EmailMessage(string to, string fullName, NotificationType type, DateTime createdAt, Payload payload)
        {
            To = to;
            FullName = fullName;
            Type = type;
            Payload = payload;
            CreatedAt = createdAt;
        }

        /// <summary>Operação para criar mensagem de e-mail.</summary>
        /// <param name="to">Destinatário do e-mail.</param>
        /// <param name="fullName">Nome completo do destinatário.</param>
        /// <param name="type">Tipo da notificação.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        /// <param name="payload">Conteúdo da notificação.</param>
        public static EmailMessage Create(string to, string fullName, NotificationType type, DateTime utcNow, Payload payload)
        {
            var email = Email.Create(to);

            if (string.IsNullOrWhiteSpace(fullName))
                throw new BadRequestException("O nome do destinatário é obrigatório.");

            if (payload is null || payload.Type != type)
                throw new BadRequestException("O conteúdo da notificação não é compatível com o tipo informado.");

            return new EmailMessage(email.Value, fullName.Trim(), type, utcNow, payload);
        }

        /// <summary>Operação para obter payload tipado da mensagem.</summary>
        public TPayload GetPayload<TPayload>() where TPayload : Payload
        {
            if (Payload is not TPayload typedPayload)
                throw new BadRequestException(
                    $"O payload do tipo '{typeof(TPayload).Name}' não é compatível com a notificação '{Type}'.");

            return typedPayload;
        }
    }
}
