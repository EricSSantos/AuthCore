namespace AuthCore.Domain.Core.Interfaces.Messaging
{
    /// <summary>
    /// Define métodos para publicação de mensagens em filas RabbitMQ.
    /// </summary>
    public interface IRabbitMqClient : IDisposable
    {
        /// <summary>
        /// Publica uma mensagem na fila especificada.
        /// </summary>
        /// <typeparam name="T">Tipo da mensagem a ser publicada.</typeparam>
        /// <param name="queueName">Nome da fila de destino.</param>
        /// <param name="message">Mensagem a ser serializada e enviada.</param>
        void Publish<T>(string queueName, T message);
    }
}
