using AuthCore.Domain.Aggregates.MessagingAggregate.Payloads;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Base;

namespace AuthCore.Domain.Aggregates.MessagingAggregate
{
    /// <summary>
    /// Representa uma notificação gerada pela aplicação.
    /// </summary>
    public sealed class Messaging : IAggregateRoot
    {
        public Guid Id { get; private set; }
        public string To { get; private set; } = null!;
        public string FullName { get; private set; } = null!;
        public MessagingType Type { get; private set; }
        public Payload? Payload { get; private set; }
        public DateTime CreatedAt { get; private set; }

        #region Constructors

        private Messaging(
            Guid id, 
            string to, 
            string fullName, 
            MessagingType type, 
            Payload? payload = null)
        {
            Id = id;
            To = to.Trim();
            FullName = fullName.Trim();
            Type = type;
            Payload = payload;
            CreatedAt = DateTime.UtcNow;
            Validate();
        }

        #endregion

        #region Factory

        /// <summary>
        /// Cria uma notificação do tipo informado.
        /// </summary>
        /// <param name="to">E-mail do destinatário.</param>
        /// <param name="fullName">Nome completo do destinatário.</param>
        /// <param name="type">Tipo da notificação.</param>
        public static Messaging Create(
            string to, 
            string fullName, 
            MessagingType type)
        {
            Payload payload;
            switch (type)
            {
                case MessagingType.ConfirmEmail:
                    payload = new ConfirmEmailPayload();
                    break;
                case MessagingType.Welcome:
                    payload = new WelcomeEmailPayload();
                    break;
                case MessagingType.ForgotPassword:
                    payload = new ForgotPasswordPayload();
                    break;
                default:
                    throw new BadRequestException("O tipo de notificação informado é inválido.");
            }

            return new Messaging(
                Guid.NewGuid(), 
                to, fullName, 
                type, 
                payload
            );
        }

        #endregion

        public TPayload GetPayload<TPayload>() where TPayload : Payload
        {
            if (Payload is not TPayload typedPayload)
                throw new BadRequestException(
                    $"O payload do tipo '{typeof(TPayload).Name}' não é compatível com a notificação '{Type}'.");

            return typedPayload;
        }

        private void Validate()
        {
            List<string> errors = new();

            Email.Validate(To);

            if (string.IsNullOrWhiteSpace(FullName))
                errors.Add("O nome do destinatário é obrigatório.");

            if (!Enum.IsDefined(typeof(MessagingType), Type))
                errors.Add("O tipo de notificação é inválido.");

            if (Payload is null)
                errors.Add("O conteúdo é obrigatório.");

            if (errors.Count > 0)
                throw new BadRequestException(errors);
        }
    }
}
