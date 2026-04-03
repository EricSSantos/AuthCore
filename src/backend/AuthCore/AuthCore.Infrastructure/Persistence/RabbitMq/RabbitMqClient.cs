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
        private readonly object _publishLock = new();
        private readonly HashSet<string> _declaredQueues = new();
        private bool _disposed;

        /// <summary>Operação para criar instância do cliente RabbitMQ.</summary>
        /// <param name="options">Configurações do RabbitMQ.</param>
        /// <param name="logger">Serviço de logging da operação.</param>
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
            _logger.LogInformation("RabbitMQ conectado.");
        }

        /// <summary>Operação para publicar mensagem na fila.</summary>
        /// <typeparam name="T">Tipo da mensagem.</typeparam>
        /// <param name="queueName">Nome da fila.</param>
        /// <param name="message">Mensagem a publicar.</param>
        public void Publish<T>(string queueName, T message)
        {
            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);

            lock (_publishLock)
            {
                if (_declaredQueues.Add(queueName))
                {
                    _channel.QueueDeclare(
                        queue: queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false
                    );
                }

                _channel.BasicPublish(
                    exchange: "",
                    routingKey: queueName,
                    basicProperties: null,
                    body: bytes
                );
            }

            _logger.LogInformation("Mensagem publicada na fila {Queue}.", queueName);
        }

        /// <summary>Operação para liberar recursos do cliente RabbitMQ.</summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
