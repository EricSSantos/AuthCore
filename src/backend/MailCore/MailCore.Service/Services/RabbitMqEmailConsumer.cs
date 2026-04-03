using MailCore.Service.Contracts;
using MailCore.Service.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace MailCore.Service.Services
{
    /// <summary>Consome a fila de e-mails do RabbitMQ.</summary>
    public sealed class RabbitMqEmailConsumer : IDisposable
    {
        private const int MaxRetry = 3;
        private static readonly TimeSpan ConnectionHealthCheckInterval = TimeSpan.FromSeconds(5);

        private readonly RabbitMqSettings _settings;
        private readonly ILogger<RabbitMqEmailConsumer> _logger;
        private readonly EmailService _mailService;
        private readonly object _lifecycleLock = new();

        private IConnection? _connection;
        private IModel? _channel;
        private RabbitMqMessageRecoveryPolicy? _messageRecoveryPolicy;
        private string? _consumerTag;
        private bool _shutdownRequested;
        private bool _disposed;
        private int _activeHandlers;
        private TaskCompletionSource<object?>? _drainCompletionSource;

        /// <summary>Inicializa o consumidor de e-mails do RabbitMQ.</summary>
        /// <param name="options">Configurações do RabbitMQ.</param>
        /// <param name="mailService">Serviço responsável pelo envio dos e-mails.</param>
        /// <param name="logger">Logger do consumidor.</param>
        public RabbitMqEmailConsumer(
            IOptions<RabbitMqSettings> options,
            EmailService mailService,
            ILogger<RabbitMqEmailConsumer> logger)
        {
            _settings = options.Value;
            _mailService = mailService;
            _logger = logger;
        }

        /// <summary>Inicia o consumo da fila configurada.</summary>
        /// <param name="cancellationToken">Token usado para interromper o consumo.</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _shutdownRequested = false;
                _drainCompletionSource = new TaskCompletionSource<object?>(
                    TaskCreationOptions.RunContinuationsAsynchronously);

                try
                {
                    await InitializeInfrastructureAsync(cancellationToken);
                    StartConsuming(cancellationToken);
                    await MonitorConnectionAsync(cancellationToken);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Encerramento solicitado para o consumidor RabbitMQ.");
                }
                finally
                {
                    RequestShutdown();
                    await WaitForDrainAsync();
                    SafeDisposeChannel();
                    SafeDisposeConnection();
                    _messageRecoveryPolicy = null;
                    Interlocked.Exchange(ref _activeHandlers, 0);
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                _logger.LogWarning(
                    "Canal/conexão RabbitMQ indisponível. Tentando reconectar em {DelaySeconds}s.",
                    ConnectionHealthCheckInterval.TotalSeconds);

                try
                {
                    await Task.Delay(ConnectionHealthCheckInterval, cancellationToken);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        /// <summary>Libera a conexão e o canal do RabbitMQ.</summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _logger.LogInformation("Encerrando conexão com RabbitMQ...");

            SafeDisposeChannel();
            SafeDisposeConnection();

            _logger.LogInformation("Conexão RabbitMQ encerrada.");
        }

        /// <summary>Cria a conexão, o canal e as filas necessárias.</summary>
        /// <param name="cancellationToken">Token usado para cancelar a inicialização.</param>
        private async Task InitializeInfrastructureAsync(CancellationToken cancellationToken)
        {
            var factory = CreateConnectionFactory();

            for (var attempt = 1; attempt <= MaxRetry; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    _logger.LogInformation(
                        "Tentando conectar ao RabbitMQ (tentativa {Attempt}/{Max}) em {Host}:{Port}...",
                        attempt,
                        MaxRetry,
                        _settings.Host,
                        _settings.Port);

                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();
                    _channel.BasicQos(0, 1, false);

                    DeclareQueues();
                    _messageRecoveryPolicy = new RabbitMqMessageRecoveryPolicy(_settings, _channel, _logger);

                    _logger.LogInformation("Conexão com RabbitMQ estabelecida com sucesso.");
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Falha ao conectar ao RabbitMQ (tentativa {Attempt}/{Max}) em {Host}:{Port}.",
                        attempt,
                        MaxRetry,
                        _settings.Host,
                        _settings.Port);

                    SafeDisposeChannel();
                    SafeDisposeConnection();

                    if (attempt == MaxRetry)
                    {
                        throw;
                    }

                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                    _logger.LogWarning(
                        "Aguardando {DelaySeconds}s antes da próxima tentativa de conexão.",
                        delay.TotalSeconds);
                    await Task.Delay(delay, cancellationToken);
                }
            }
        }

        /// <summary>Cria a fábrica de conexão do RabbitMQ.</summary>
        /// <returns>Instância configurada da fábrica.</returns>
        private ConnectionFactory CreateConnectionFactory()
        {
            return new ConnectionFactory
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                UserName = _settings.User,
                Password = _settings.Password,
                DispatchConsumersAsync = true
            };
        }

        /// <summary>Declara as filas usadas pelo consumidor.</summary>
        private void DeclareQueues()
        {
            EnsureChannel();

            _channel!.QueueDeclare(
                queue: _settings.EmailQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.QueueDeclare(
                queue: _settings.DeadLetterQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _logger.LogInformation(
                "Filas declaradas com sucesso: {EmailQueue}, {DeadLetterQueue}",
                _settings.EmailQueue,
                _settings.DeadLetterQueue);
        }

        /// <summary>Monitora a saúde da conexão até que seja necessário reconectar.</summary>
        /// <param name="cancellationToken">Token usado para interrupção cooperativa.</param>
        private async Task MonitorConnectionAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_connection is null || !_connection.IsOpen)
                {
                    _logger.LogWarning("Conexão RabbitMQ encerrada. Reconexão será iniciada.");
                    return;
                }

                if (_channel is null || !_channel.IsOpen)
                {
                    _logger.LogWarning("Canal RabbitMQ encerrado. Reconexão será iniciada.");
                    return;
                }

                await Task.Delay(ConnectionHealthCheckInterval, cancellationToken);
            }
        }

        /// <summary>Inicia o consumo com o canal já configurado.</summary>
        /// <param name="cancellationToken">Token usado para controlar o encerramento.</param>
        private void StartConsuming(CancellationToken cancellationToken)
        {
            EnsureChannel();
            EnsureMessageRecoveryPolicy();
            EnsureDrainCompletionSource();

            _logger.LogInformation(
                "Iniciando o consumo de mensagens da fila {Queue}.",
                _settings.EmailQueue);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += HandleReceivedAsync;

            _consumerTag = _channel!.BasicConsume(
                queue: _settings.EmailQueue,
                autoAck: false,
                consumer: consumer);

            _logger.LogInformation(
                "RabbitMQ iniciado com sucesso e aguardando mensagens. Token cancelável configurado: {IsCancellationRequested}.",
                cancellationToken.IsCancellationRequested);
        }

        /// <summary>Processa as mensagens recebidas pela fila.</summary>
        /// <param name="sender">Origem do evento.</param>
        /// <param name="ea">Mensagem recebida do RabbitMQ.</param>
        /// <returns>Tarefa de processamento da mensagem.</returns>
        private async Task HandleReceivedAsync(object? sender, BasicDeliverEventArgs ea)
        {
            Interlocked.Increment(ref _activeHandlers);

            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                if (!TryDeserializeEmail(json, out var email, out var invalidReason, out var invalidException))
                {
                    ApplyDisposition(
                        ea.DeliveryTag,
                        _messageRecoveryPolicy.RouteInvalidMessage(json, invalidReason, invalidException));
                    return;
                }

                using (_logger.BeginScope(new Dictionary<string, object?>
                {
                    ["EmailId"] = email.Id,
                    ["EmailType"] = email.Type
                }))
                {
                    _logger.LogInformation(
                        "Processando o e-mail {EmailId} do tipo {EmailType}.",
                        email.Id,
                        email.Type);

                    await _mailService.SendEmail(email);

                    _logger.LogInformation("E-mail processado e enviado com sucesso.");
                }

                ApplyDisposition(ea.DeliveryTag, RabbitMqMessageDisposition.Ack);
            }
            catch (Exception ex)
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var retryCount = _messageRecoveryPolicy!.GetRetryCount(ea);

                ApplyDisposition(
                    ea.DeliveryTag,
                    _messageRecoveryPolicy.RouteProcessingFailure(json, ex, retryCount));
            }
            finally
            {
                ReleaseHandler();
            }
        }

        /// <summary>Aplica a confirmação definida para a mensagem.</summary>
        /// <param name="deliveryTag">Identificador da mensagem no broker.</param>
        /// <param name="disposition">Ação esperada para a mensagem.</param>
        private void ApplyDisposition(ulong deliveryTag, RabbitMqMessageDisposition disposition)
        {
            try
            {
                EnsureChannel();

                switch (disposition)
                {
                    case RabbitMqMessageDisposition.Ack:
                        _channel!.BasicAck(deliveryTag, multiple: false);
                        break;
                    case RabbitMqMessageDisposition.NackRequeue:
                        _channel!.BasicNack(deliveryTag, multiple: false, requeue: true);
                        break;
                    case RabbitMqMessageDisposition.NackDiscard:
                        _channel!.BasicNack(deliveryTag, multiple: false, requeue: false);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(disposition), disposition, "Ação de confirmação inválida.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Falha ao aplicar a confirmação {Disposition} para a mensagem {DeliveryTag}.",
                    disposition,
                    deliveryTag);
            }
        }

        /// <summary>Fecha o consumo iniciado no canal atual.</summary>
        private void RequestShutdown()
        {
            lock (_lifecycleLock)
            {
                _shutdownRequested = true;

                if (_channel is not null && !string.IsNullOrWhiteSpace(_consumerTag))
                {
                    try
                    {
                        _channel.BasicCancel(_consumerTag);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Falha ao cancelar o consumidor RabbitMQ de forma explícita.");
                    }
                    finally
                    {
                        _consumerTag = null;
                    }
                }

                TryCompleteDrainCompletion();
            }
        }

        /// <summary>Registra que o processamento de uma mensagem terminou.</summary>
        private void ReleaseHandler()
        {
            if (Interlocked.Decrement(ref _activeHandlers) != 0)
            {
                return;
            }

            TryCompleteDrainCompletion();
        }

        /// <summary>Cria um ponto de espera para o encerramento do processamento em andamento.</summary>
        private void EnsureDrainCompletionSource()
        {
            _drainCompletionSource ??= new TaskCompletionSource<object?>(
                TaskCreationOptions.RunContinuationsAsynchronously);
        }

        /// <summary>Finaliza a espera de desligamento quando não há mensagens em processamento.</summary>
        private void TryCompleteDrainCompletion()
        {
            if (!_shutdownRequested || _drainCompletionSource is null || Volatile.Read(ref _activeHandlers) != 0)
            {
                return;
            }

            _drainCompletionSource.TrySetResult(null);
        }

        /// <summary>Aguarda a liberação das mensagens que já estavam em processamento.</summary>
        private async Task WaitForDrainAsync()
        {
            EnsureDrainCompletionSource();
            TryCompleteDrainCompletion();

            if (_drainCompletionSource is not null)
            {
                await _drainCompletionSource.Task;
            }
        }

        /// <summary>Tenta desserializar a mensagem recebida.</summary>
        /// <param name="json">Conteúdo bruto da mensagem.</param>
        /// <param name="email">Mensagem desserializada, quando válida.</param>
        /// <param name="invalidReason">Motivo da invalidação, quando aplicável.</param>
        /// <param name="exception">Exceção associada ao erro, quando existir.</param>
        /// <returns><c>true</c> quando a mensagem é válida.</returns>
        private bool TryDeserializeEmail(
            string json,
            out Email? email,
            out string invalidReason,
            out Exception? exception)
        {
            email = null;
            invalidReason = string.Empty;
            exception = null;

            try
            {
                email = JsonSerializer.Deserialize<Email>(json);
            }
            catch (JsonException ex)
            {
                invalidReason = "Falha ao desserializar o JSON recebido.";
                exception = ex;
                return false;
            }
            catch (NotSupportedException ex)
            {
                invalidReason = "O payload recebido possui um formato incompatível.";
                exception = ex;
                return false;
            }

            if (email is null)
            {
                invalidReason = "A mensagem desserializada ficou nula.";
                return false;
            }

            if (email.Id == Guid.Empty)
            {
                invalidReason = "A mensagem recebida não possui identificador válido.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(email.To))
            {
                invalidReason = "A mensagem recebida não possui destinatário.";
                return false;
            }

            try
            {
                email.Validate();
            }
            catch (Exception ex)
            {
                invalidReason = ex.Message;
                exception = ex;
                return false;
            }

            return true;
        }

        /// <summary>Garante que o canal de RabbitMQ foi inicializado.</summary>
        private void EnsureChannel()
        {
            if (_channel is null)
            {
                throw new InvalidOperationException("O canal do RabbitMQ ainda não foi inicializado.");
            }
        }

        /// <summary>Garante que a política de recuperação foi inicializada.</summary>
        private void EnsureMessageRecoveryPolicy()
        {
            if (_messageRecoveryPolicy is null)
            {
                throw new InvalidOperationException("A política de recuperação do RabbitMQ ainda não foi inicializada.");
            }
        }

        /// <summary>Libera o canal do RabbitMQ com segurança.</summary>
        private void SafeDisposeChannel()
        {
            try
            {
                _channel?.Close();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao encerrar o canal do RabbitMQ.");
            }
            finally
            {
                _channel?.Dispose();
                _channel = null;
            }
        }

        /// <summary>Libera a conexão do RabbitMQ com segurança.</summary>
        private void SafeDisposeConnection()
        {
            try
            {
                _connection?.Close();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao encerrar a conexão do RabbitMQ.");
            }
            finally
            {
                _connection?.Dispose();
                _connection = null;
            }
        }
    }
}
