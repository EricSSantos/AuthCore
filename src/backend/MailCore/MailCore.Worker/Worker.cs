using MailCore.Service.Services;

namespace MailCore.Worker
{
    /// <summary>Inicia o consumidor de e-mails do MailCore.</summary>
    public sealed class Worker : BackgroundService
    {
        private readonly RabbitMqEmailConsumer _consumer;

        /// <summary>Inicializa o worker do MailCore.</summary>
        public Worker(RabbitMqEmailConsumer consumer)
        {
            _consumer = consumer;
        }

        /// <summary>Executa o consumo da fila enquanto o serviço estiver ativo.</summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.StartAsync(stoppingToken);
        }
    }
}
