using System.ComponentModel.DataAnnotations;

namespace MailCore.Service.Settings
{
    /// <summary>Configurações do SMTP.</summary>
    public sealed class SmtpSettings
    {
        [Required(ErrorMessage = "O host do SMTP é obrigatório.")]
        public string Host { get; set; } = string.Empty;

        [Range(1, 65535, ErrorMessage = "A porta do SMTP deve estar entre 1 e 65535.")]
        public int Port { get; set; }

        [Required(ErrorMessage = "O usuário do SMTP é obrigatório.")]
        public string User { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha do SMTP é obrigatória.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "O endereço de origem do SMTP é obrigatório.")]
        [EmailAddress(ErrorMessage = "O endereço de origem do SMTP deve ser um e-mail válido.")]
        public string FromEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome de origem do SMTP é obrigatório.")]
        public string FromName { get; set; } = string.Empty;

        [Range(1, 300, ErrorMessage = "O timeout do SMTP deve estar entre 1 e 300 segundos.")]
        public int TimeoutSeconds { get; set; } = 30;

        public bool EnableSsl { get; set; } = true;
    }
}
