using AuthCore.Domain.Shared;
using System.Text.Json;

namespace AuthCore.Domain.Aggregates.EmailAggregate
{
    public sealed class Email : Entity
    {
        public string To { get; private set; } = string.Empty;
        public EmailType Type { get; private set; }
        public string Payload { get; private set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; private set; }

        private Email() { }

        public Email(string to, EmailType type, object data)
        {
            To = to;
            Type = type;
            Payload = JsonSerializer.Serialize(data);
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public string ToJson() => JsonSerializer.Serialize(this);
    }
}
