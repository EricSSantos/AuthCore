using MailCore.Service.Settings;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MailCore.Service.Services
{
    /// <summary>Define a ação de confirmação esperada para a mensagem consumida.</summary>
    internal enum RabbitMqMessageDisposition
    {
        Ack = 0,
        NackRequeue = 1,
        NackDiscard = 2
    }

    /// <summary>Encapsula a política de retry e encaminhamento para DLQ.</summary>
    internal sealed class RabbitMqMessageRecoveryPolicy
    {
        private const int MaxRetry = 3;
        private const string RetryHeader = "x-retry-count";
        private const string ErrorHeader = "x-error";

        private readonly RabbitMqSettings _settings;
        private readonly IModel _channel;
        private readonly ILogger _logger;

        /// <summary>Inicializa a política de recuperação de mensagens.</summary>
        /// <param name="settings">Configurações do RabbitMQ.</param>
        /// <param name="channel">Canal usado para republicação.</param>
        /// <param name="logger">Logger compartilhado com o consumidor.</param>
        public RabbitMqMessageRecoveryPolicy(
            RabbitMqSettings settings,
            IModel channel,
            ILogger logger)
        {
            _settings = settings;
            _channel = channel;
            _logger = logger;
        }

        /// <summary>Obtém a quantidade de reprocessamentos da mensagem recebida.</summary>
        /// <param name="delivery">Mensagem entregue pelo RabbitMQ.</param>
        /// <returns>Quantidade de retries já executados.</returns>
        public int GetRetryCount(BasicDeliverEventArgs delivery)
        {
            if (delivery.BasicProperties?.Headers is null)
            {
                return 0;
            }

            if (!delivery.BasicProperties.Headers.TryGetValue(RetryHeader, out var retryHeader))
            {
                return 0;
            }

            return retryHeader switch
            {
                byte[] retryBytes when int.TryParse(Encoding.UTF8.GetString(retryBytes), out var retryCount) => retryCount,
                int retryCount => retryCount,
                long retryLong when retryLong >= int.MinValue && retryLong <= int.MaxValue => (int)retryLong,
                _ => 0
            };
        }

        /// <summary>Encaminha uma mensagem inválida para a fila de dead-letter.</summary>
        /// <param name="json">Conteúdo bruto recebido.</param>
        /// <param name="reason">Motivo da invalidação.</param>
        /// <param name="exception">Exceção original, quando existir.</param>
        /// <returns>A ação de confirmação que deve ser aplicada no consumo.</returns>
        public RabbitMqMessageDisposition RouteInvalidMessage(string json, string reason, Exception? exception)
        {
            _logger.LogWarning(
                exception,
                "Mensagem inválida recebida. Motivo: {Reason}. Enviando para DLQ.",
                reason);

            if (TryPublishToDeadLetter(json, reason))
            {
                _logger.LogInformation("Mensagem inválida encaminhada para a DLQ com sucesso.");
                return RabbitMqMessageDisposition.Ack;
            }

            _logger.LogError(
                "Falha ao encaminhar mensagem inválida para a DLQ. A mensagem será reenfileirada para preservar a entrega.");
            return RabbitMqMessageDisposition.NackRequeue;
        }

        /// <summary>Aplica a política de retry ou DLQ para uma falha de processamento.</summary>
        /// <param name="json">Conteúdo bruto da mensagem.</param>
        /// <param name="exception">Exceção de processamento.</param>
        /// <param name="retryCount">Quantidade atual de retries da mensagem.</param>
        /// <returns>A ação de confirmação que deve ser aplicada no consumo.</returns>
        public RabbitMqMessageDisposition RouteProcessingFailure(string json, Exception exception, int retryCount)
        {
            var nextRetryCount = retryCount + 1;

            if (nextRetryCount >= MaxRetry)
            {
                _logger.LogError(
                    exception,
                    "Falha ao processar e-mail após {RetryCount} tentativas. Encaminhando para DLQ.",
                    nextRetryCount);

                if (TryPublishToDeadLetter(json, exception.Message))
                {
                    _logger.LogInformation("Mensagem encaminhada para a DLQ após esgotar as tentativas.");
                    return RabbitMqMessageDisposition.Ack;
                }

                _logger.LogError(
                    "Falha ao encaminhar mensagem para a DLQ após esgotar as tentativas. A mensagem permanecerá disponível para novo consumo.");
                return RabbitMqMessageDisposition.NackRequeue;
            }

            _logger.LogError(
                exception,
                "Erro ao processar e-mail. Tentativa {RetryCount}/{MaxRetry}. Reenviando para a fila.",
                nextRetryCount,
                MaxRetry);

            if (TryPublishRetry(json, nextRetryCount))
            {
                _logger.LogInformation(
                    "Mensagem reenviada para a fila com retry {RetryCount}/{MaxRetry}.",
                    nextRetryCount,
                    MaxRetry);
                return RabbitMqMessageDisposition.Ack;
            }

            _logger.LogError(
                "Falha ao reenviar mensagem para retry. A mensagem permanecerá disponível para novo consumo.");
            return RabbitMqMessageDisposition.NackRequeue;
        }

        private bool TryPublishRetry(string json, int retryCount)
        {
            try
            {
                var properties = CreatePersistentProperties();
                properties.Headers = new Dictionary<string, object>
                {
                    { RetryHeader, Encoding.UTF8.GetBytes(retryCount.ToString()) }
                };

                _channel.BasicPublish(
                    exchange: string.Empty,
                    routingKey: _settings.EmailQueue,
                    basicProperties: properties,
                    body: Encoding.UTF8.GetBytes(json));

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao publicar a mensagem de retry na fila {Queue}.", _settings.EmailQueue);
                return false;
            }
        }

        private bool TryPublishToDeadLetter(string json, string errorMessage)
        {
            try
            {
                var properties = CreatePersistentProperties();
                properties.Headers = new Dictionary<string, object>
                {
                    { ErrorHeader, Encoding.UTF8.GetBytes(errorMessage) }
                };

                _channel.BasicPublish(
                    exchange: string.Empty,
                    routingKey: _settings.DeadLetterQueue,
                    basicProperties: properties,
                    body: Encoding.UTF8.GetBytes(json));

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Falha ao publicar a mensagem na DLQ {DeadLetterQueue}.",
                    _settings.DeadLetterQueue);
                return false;
            }
        }

        private IBasicProperties CreatePersistentProperties()
        {
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            return properties;
        }
    }
}
