namespace AuthCore.Domain.Core.Settings
{
    /// <summary>
    /// Define as configurações de conexão e filas utilizadas pelo RabbitMQ.
    /// </summary>
    public sealed class RabbitMqSettings
    {
        /// <summary>
        /// Endereço do host do servidor RabbitMQ.
        /// </summary>
        public string Host { get; init; } = string.Empty;

        /// <summary>
        /// Porta de conexão com o servidor RabbitMQ.
        /// </summary>
        public int Port { get; init; }

        /// <summary>
        /// Nome de usuário utilizado na autenticação.
        /// </summary>
        public string User { get; init; } = string.Empty;

        /// <summary>
        /// Senha utilizada na autenticação.
        /// </summary>
        public string Password { get; init; } = string.Empty;

        /// <summary>
        /// Nome da fila principal de e-mails.
        /// </summary>
        public string EmailQueue { get; init; } = string.Empty;

        /// <summary>
        /// Nome da fila de mensagens inválidas (Dead Letter Queue).
        /// </summary>
        public string DeadLetterQueue { get; init; } = string.Empty;
    }
}
