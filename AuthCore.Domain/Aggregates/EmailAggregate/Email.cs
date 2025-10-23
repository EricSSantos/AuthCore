using AuthCore.Domain.Aggregates.EmailAggregate.Payloads;
using AuthCore.Domain.Aggregates.EmailAggregate.Strategies;
using AuthCore.Domain.Commons.Exceptions;
using AuthCore.Domain.Shared;

namespace AuthCore.Domain.Aggregates.EmailAggregate
{
    /// <summary>
    /// Estrutura base para qualquer e-mail,
    /// contendo informações comuns como destinatário, tipo, payload e data de criação.
    /// </summary>
    public abstract class Email : Entity
    {
        #region Properties

        public string To { get; private set; }
        public string FullName { get; private set; }
        public EmailType Type { get; private set; }
        public EmailPayload? Payload { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        #endregion

        #region Constructors

        private Email()
        { }

        protected Email(string to, string fullName, EmailType type, EmailPayload? payload = null)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new DomainException("O email do destinatário não pode ser vazio.");
            if (string.IsNullOrWhiteSpace(fullName))
                throw new DomainException("O nome do destinatário não pode ser vazio.");

            To = to.Trim();
            FullName = fullName.Trim();
            Type = type;
            Payload = payload;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        #endregion

        #region Factory

        public static Email Create(string to, string fullName, EmailType type, EmailPayload? payload = null)
        {
            return type switch
            {
                EmailType.Welcome => WelcomeEmail.Create(to, fullName),
                EmailType.ForgotPassword => ForgotPasswordEmail.Create(to, fullName),
                _ => throw new DomainException("Tipo de email desconhecido.")
            };
        }

        #endregion
    }
}
