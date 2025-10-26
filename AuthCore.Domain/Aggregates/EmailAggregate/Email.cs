using AuthCore.Domain.Aggregates.EmailAggregate.Payloads;
using AuthCore.Domain.Aggregates.EmailAggregate.Strategies;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Base;

namespace AuthCore.Domain.Aggregates.EmailAggregate
{
    /// <summary>
    /// Representa um e-mail transacional gerado pela aplicação.
    /// </summary>
    public abstract class Email : IAggregateRoot
    {
        #region Properties

        /// <summary>
        /// Identificador único do e-mail.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Endereço de e-mail do destinatário.
        /// </summary>
        public string To { get; private set; }

        /// <summary>
        /// Nome completo do destinatário.
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// Tipo do e-mail (ex: Boas-vindas, Recuperação de Senha).
        /// </summary>
        public EmailType Type { get; private set; }

        /// <summary>
        /// Dados adicionais específicos do tipo de e-mail.
        /// </summary>
        public EmailPayload? Payload { get; private set; }

        /// <summary>
        /// Data e hora em que o e-mail foi criado.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        #endregion

        #region Constructors

        protected Email(
            string to,
            string fullName,
            EmailType type,
            EmailPayload? payload = null)
        {
            Id = Guid.NewGuid();
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
        /// Cria uma instância de e-mail de acordo com o tipo especificado.
        /// </summary>
        public static Email Create(
            string to,
            string fullName,
            EmailType type,
            EmailPayload? payload = null)
        {
            return type switch
            {
                EmailType.Welcome => WelcomeEmail.Create(to, fullName),
                EmailType.ForgotPassword => ForgotPasswordEmail.Create(to, fullName),
                _ => throw new BadRequestException("O tipo de e-mail informado é inválido.")
            };
        }

        #endregion

        #region Validation

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(To))
                throw new BadRequestException("O e-mail do destinatário é obrigatório.");
            if (string.IsNullOrWhiteSpace(FullName))
                throw new BadRequestException("O nome do destinatário é obrigatório.");
        }

        #endregion
    }
}
