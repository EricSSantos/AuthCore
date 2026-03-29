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

        /// <summary>Operação para criar instância do serviço de envio de notificações.</summary>
        /// <param name="client">Cliente de publicação RabbitMQ.</param>
        /// <param name="options">Configurações do RabbitMQ.</param>
        /// <param name="logger">Serviço de logging da operação.</param>
        public EmailSender(
            IRabbitMqClient client,
            IOptions<RabbitMqSettings> options,
            ILogger<EmailSender> logger)
        {
            _client = client;
            _settings = options.Value;
            _logger = logger;
        }

        /// <summary>Operação para enviar mensagem por e-mail.</summary>
        /// <param name="message">Mensagem a ser enviada.</param>
        public Task SendAsync(EmailMessage message)
        {
            var document = new EmailDocument
            {
                Id = Guid.NewGuid(),
                To = message.To,
                FullName = message.FullName,
                Type = message.Type,
                Payload = message.Payload,
                CreatedAt = message.CreatedAt
            };

            _client.Publish(_settings.EmailQueue, document);

            _logger.LogInformation("E-mail enfileirado para {To}.", message.To);
            return Task.CompletedTask;
        }
    }
}
