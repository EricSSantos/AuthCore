using AuthCore.Domain.Aggregates.EmailAggregate;
using AuthCore.Domain.Commons.Interfaces.Messaging;
using AuthCore.Domain.Commons.Settings;
using AuthCore.Infrastructure.Messaging.RabbitMq.Mappings;
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

        public Task Send(Email email)
        {
            var document = new EmailDocument
            {
                Id = email.Id,
                To = email.To,
                FullName = email.FullName,
                Type = email.Type,
                Payload = email.Payload,
                CreatedAt = email.CreatedAt
            };

            _client.Publish(_settings.EmailQueue, document);

            _logger.LogInformation("E-mail {EmailId} enfileirado.", email.Id);

            return Task.CompletedTask;
        }

    }
}
