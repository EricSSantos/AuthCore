using AuthCore.Domain.Aggregates.EmailAggregate;
using AuthCore.Domain.Commons.Interfaces.Messaging;
using AuthCore.Domain.Commons.Settings;
using AuthCore.Infrastructure.Messaging.RabbitMq.Documents;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthCore.Infrastructure.Messaging.RabbitMq
{
    public sealed class EmailService : IEmailService
    {
        private readonly IRabbitMqClient _client;
        private readonly RabbitMqSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IRabbitMqClient client,
            IOptions<RabbitMqSettings> options,
            ILogger<EmailService> logger)
        {
            _client = client;
            _settings = options.Value;
            _logger = logger;
        }

        public Task Send(string to, string fullName, EmailType type, object? data = null)
        {
            var email = Email.Create(
                to,
                fullName,
                type,
                data ?? new { }
            );

            var document = new EmailDocument
            {
                To = email.To,
                FullName = email.FullName,
                Type = email.Type,
                Content = email.Content,
                CreatedAt = email.CreatedAt.UtcDateTime
            };

            _client.Publish(_settings.EmailQueue, document);
            _logger.LogInformation("E-mail {Type} enfileirado para {To}", type, to);

            return Task.CompletedTask;
        }
    }
}
