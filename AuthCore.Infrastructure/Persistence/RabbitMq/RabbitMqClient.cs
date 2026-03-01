using AuthCore.Domain.Core.Interfaces.Infrastructure.Notifications;
using AuthCore.Domain.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace AuthCore.Infrastructure.Persistence.RabbitMq
{
    /// <summary>Representa cliente RabbitMQ para publicação de mensagens.</summary>
    public sealed class RabbitMqClient : IRabbitMqClient
    {
        private readonly RabbitMqSettings _settings;
        private readonly ILogger<RabbitMqClient> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqClient(
            IOptions<RabbitMqSettings> options, 
            ILogger<RabbitMqClient> logger)
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

            _logger.LogInformation("RabbitMQ conectado");
        }

        public void Publish<T>(string queueName, T message)
        {
            _channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: null,
                body: bytes
            );

            _logger.LogInformation("Mensagem publicada na fila {Queue}", queueName);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
