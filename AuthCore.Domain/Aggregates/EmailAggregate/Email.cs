using AuthCore.Domain.Aggregates.EmailAggregate.Payloads;
using AuthCore.Domain.Commons.Exceptions;
using AuthCore.Domain.Shared;

namespace AuthCore.Domain.Aggregates.EmailAggregate
{
    /// <summary>
    /// Representa a estrutura base para qualquer e-mail gerado no domínio,
    /// contendo informações comuns como destinatário, tipo, payload e data de criação.
    /// Implementações concretas devem definir o tipo e o conteúdo específico do e-mail.
    /// </summary>
    public abstract class Email : Entity
    {
        public string To { get; private set; }
        public string FullName { get; private set; }
        public EmailType Type { get; private set; }
        public EmailPayload? Payload { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

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
    }
}
