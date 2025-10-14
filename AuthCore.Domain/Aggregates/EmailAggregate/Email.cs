using AuthCore.Domain.Shared;
using System.Text.Json;

namespace AuthCore.Domain.Aggregates.EmailAggregate
{
    public sealed class Email : Entity
    {
        #region Properties

        public string To { get; private set; } = string.Empty;
        public string FullName { get; private set; } = string.Empty;
        public EmailType Type { get; private set; }
        public string Content { get; private set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

        #endregion

        #region Constructors

        private Email() { }

        private Email(string to, string fullName, EmailType type, string content)
        {
            To = to;
            FullName = fullName;
            Type = type;
            Content = content;
        }

        #endregion

        #region Factory

        public static Email Create(string to, string fullName, EmailType type, object data)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("O email do destinatário não pode ser vazio.");

            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("O nome destinatário não pode ser vazio.");

            var serializedData = JsonSerializer.Serialize(data ?? new { });

            return new Email(
                to: to.Trim(),
                fullName: fullName.Trim(),
                type: type,
                content: serializedData
            );
        }

        #endregion
    }
}
