using AuthCore.Domain.Aggregates.EmailAggregate.Payloads;
using AuthCore.Domain.Aggregates.EmailAggregate.Strategies;
using AuthCore.Domain.Commons.Exceptions;
using AuthCore.Domain.Shared;

namespace AuthCore.Domain.Aggregates.EmailAggregate
{
    public abstract class Email : Entity
    {
        #region Properties

        public string To { get; private set; }
        public string FullName { get; private set; }
        public EmailType Type { get; private set; }
        public EmailPayload? Payload { get; private set; }
        public DateTime CreatedAt { get; private set; }

        #endregion

        #region Constructors

        protected Email() { }

        protected Email(
            string to,
            string fullName,
            EmailType type,
            EmailPayload? payload = null)
        {
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
        /// Cria uma instância de e-mail de acordo com o tipo.
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

        #region Private Methods

        private void Validate()
        {
            var validate = Validator();

            if (string.IsNullOrWhiteSpace(To))
                validate.AddError("O e-mail do destinatário é obrigatório.");
            if (string.IsNullOrWhiteSpace(FullName))
                validate.AddError("O nome do destinatário é obrigatório.");

            validate.ThrowIfInvalid();
        }

        #endregion
    }
}
