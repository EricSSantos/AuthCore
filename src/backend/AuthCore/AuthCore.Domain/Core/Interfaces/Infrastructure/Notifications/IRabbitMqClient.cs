namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Notifications
{
    /// <summary>Define operações para publicação de mensagens em RabbitMQ.</summary>
    public interface IRabbitMqClient : IDisposable
    {
        /// <summary>Operação para publicar mensagem na fila.</summary>
        /// <typeparam name="T">Tipo da mensagem.</typeparam>
        /// <param name="queueName">Nome da fila.</param>
        /// <param name="message">Mensagem a publicar.</param>
        void Publish<T>(string queueName, T message);
    }
}
