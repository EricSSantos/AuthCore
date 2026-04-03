using System.ComponentModel.DataAnnotations;

namespace MailCore.Service.Settings
{
    /// <summary>Configurações do RabbitMQ.</summary>
    public sealed class RabbitMqSettings
    {
        [Required(ErrorMessage = "O host do RabbitMQ é obrigatório.")]
        public string Host { get; set; } = string.Empty;

        [Range(1, 65535, ErrorMessage = "A porta do RabbitMQ deve estar entre 1 e 65535.")]
        public int Port { get; set; }

        [Required(ErrorMessage = "O usuário do RabbitMQ é obrigatório.")]
        public string User { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha do RabbitMQ é obrigatória.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "A fila de e-mails é obrigatória.")]
        public string EmailQueue { get; set; } = string.Empty;

        [Required(ErrorMessage = "A fila de dead-letter é obrigatória.")]
        public string DeadLetterQueue { get; set; } = string.Empty;
    }
}
