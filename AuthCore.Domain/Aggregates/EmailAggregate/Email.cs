using AuthCore.Domain.Aggregates.EmailAggregate.Payloads;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Base;

namespace AuthCore.Domain.Aggregates.EmailAggregate
{
    /// <summary>
    /// Representa um e-mail transacional gerado pela aplicação.
    /// </summary>
    public sealed class Email : IAggregateRoot
    {
        #region Properties

        /// <summary>
        /// Identifica o e-mail de forma única.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Armazena o endereço de e-mail do destinatário.
        /// </summary>
        public string To { get; private set; }

        /// <summary>
        /// Armazena o nome completo do destinatário.
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// Define o tipo do e-mail (ex: Boas-vindas, Verificação, Recuperação de Senha).
        /// </summary>
        public EmailType Type { get; private set; }

        /// <summary>
        /// Contém dados adicionais específicos do tipo de e-mail.
        /// </summary>
        public EmailPayload? Payload { get; private set; }

        /// <summary>
        /// Registra a data de criação do e-mail (UTC).
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        #endregion

        #region Constructors

        private Email(string to, string fullName, EmailType type, EmailPayload? payload = null)
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
        /// Cria um e-mail do tipo especificado.
        /// </summary>
        /// <param name="to">Endereço de e-mail do destinatário.</param>
        /// <param name="fullName">Nome completo do destinatário.</param>
        /// <param name="type">Tipo de e-mail a ser criado.</param>
        /// <returns>Instância concreta de <see cref="Email"/>.</returns>
        /// <exception cref="BadRequestException">Lançada quando o tipo de e-mail é inválido.</exception>
        public static Email Create(string to, string fullName, EmailType type)
        {
            EmailPayload payload;

            switch (type)
            {
                case EmailType.Welcome:
                    payload = new WelcomeEmailPayload();
                    break;
                case EmailType.ConfirmEmail:
                    payload = new ConfirmEmailPayload();
                    break;
                case EmailType.ForgotPassword:
                    payload = new ForgotPasswordPayload();
                    break;
                default:
                    throw new BadRequestException("O tipo de e-mail informado é inválido.");
            }

            return new Email(to, fullName, type, payload);
        }

        #endregion

        #region Behavior

        /// <summary>
        /// Retorna o payload convertido para o tipo especificado.
        /// </summary>
        /// <typeparam name="TPayload">Tipo esperado do payload.</typeparam>
        /// <returns>Instância convertida do payload.</returns>
        /// <exception cref="InvalidOperationException">
        /// Lançada quando o payload atual não é compatível com o tipo solicitado.
        /// </exception>
        public TPayload GetPayload<TPayload>() where TPayload : EmailPayload
        {
            if (Payload is not TPayload typedPayload)
                throw new BadRequestException($"O payload do tipo '{typeof(TPayload).Name}' não é compatível com o e-mail '{Type}'.");

            return typedPayload;
        }

        #endregion

        #region Validation

        /// <summary>
        /// Valida os dados obrigatórios do e-mail.
        /// </summary>
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
