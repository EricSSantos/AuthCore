using AuthCore.Application.Services.Interfaces;
using AuthCore.Domain.Aggregates.EmailAggregate;
using AuthCore.Domain.Commons.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;

namespace AuthCore.Infrastructure.Messaging.RabbitMq
{
    public sealed class EmailService : IEmailService, IDisposable
    {
        private readonly RabbitMqSettings _settings;
        private readonly ILogger<EmailService> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public EmailService(IOptions<RabbitMqSettings> options, ILogger<EmailService> logger)
        {
            _settings = options.Value;
            _logger = logger;

            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                UserName = _settings.User,
                Password = _settings.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _logger.LogInformation("RabbitMQ conectado em {Host}:{Port}", _settings.Host, _settings.Port);
        }

        public Task SendAsync(string to, EmailType type, object? data = null)
        {
            var email = new Email(to, type, data ?? new { });
            var bytes = Encoding.UTF8.GetBytes(email.ToJson());

            _channel.QueueDeclare(
                queue: _settings.EmailQueue,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            _channel.BasicPublish(
                exchange: "",
                routingKey: _settings.EmailQueue,
                basicProperties: null,
                body: bytes
            );

            _logger.LogInformation("E-mail {Type} enfileirado para {To}", type, to);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
