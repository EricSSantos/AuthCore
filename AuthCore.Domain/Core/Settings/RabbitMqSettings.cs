namespace AuthCore.Domain.Core.Settings
{
    /// <summary>
    /// Define configurações de conexão e filas do RabbitMQ.
    /// </summary>
    public sealed class RabbitMqSettings
    {
        /// <summary>
        /// Define o host do servidor RabbitMQ.
        /// </summary>
        public string Host { get; init; } = string.Empty;

        /// <summary>
        /// Define a porta de conexão.
        /// </summary>
        public int Port { get; init; }

        /// <summary>
        /// Define o usuário de autenticação.
        /// </summary>
        public string User { get; init; } = string.Empty;

        /// <summary>
        /// Define a senha de autenticação.
        /// </summary>
        public string Password { get; init; } = string.Empty;

        /// <summary>
        /// Define a fila principal de e-mails.
        /// </summary>
        public string EmailQueue { get; init; } = string.Empty;

        /// <summary>
        /// Define a fila de mensagens inválidas (DLQ).
        /// </summary>
        public string DeadLetterQueue { get; init; } = string.Empty;
    }
}
