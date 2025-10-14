namespace AuthCore.Domain.Commons.Interfaces.Messaging
{
    public interface IRabbitMqClient : IDisposable
    {
        /// <summary>
        /// Publica uma mensagem na fila especificada.
        /// </summary>
        /// <typeparam name="T">Tipo da mensagem a ser publicada.</typeparam>
        /// <param name="queueName">Nome da fila onde a mensagem será publicada.</param>
        /// <param name="message">Objeto da mensagem que será serializado e enviado.</param>
        void Publish<T>(string queueName, T message);
    }
}
