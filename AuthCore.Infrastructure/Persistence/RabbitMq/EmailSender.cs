using AuthCore.Domain.Aggregates.Notifications;
using AuthCore.Domain.Aggregates.Notifications.Interfaces;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Notifications;
using AuthCore.Domain.Core.Settings;
using AuthCore.Infrastructure.Persistence.RabbitMq.Mappings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthCore.Infrastructure.Persistence.RabbitMq
{
    /// <summary>Representa serviço de envio de notificações por RabbitMQ.</summary>
    public sealed class EmailSender : IEmailSender
    {
        private readonly IRabbitMqClient _client;
        private readonly RabbitMqSettings _settings;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(
            IRabbitMqClient client,
            IOptions<RabbitMqSettings> options,
            ILogger<EmailSender> logger)
        {
            _client = client;
            _settings = options.Value;
            _logger = logger;
        }

        public Task SendAsync(Notification notification)
        {
            var document = new EmailDocument
            {
                Id = notification.Id,
                To = notification.To,
                FullName = notification.FullName,
                Type = notification.Type,
                Payload = notification.Payload,
                CreatedAt = notification.CreatedAt
            };

            _client.Publish(_settings.EmailQueue, document);

            _logger.LogInformation("E-mail {EmailId} enfileirado.", notification.Id);

            return Task.CompletedTask;
        }
    }
}
