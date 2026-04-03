using System.ComponentModel.DataAnnotations;

namespace MailCore.Service.Settings
{
    /// <summary>Configurações da aplicação.</summary>
    public sealed class ApplicationSettings
    {
        [Required(ErrorMessage = "O nome da aplicação é obrigatório.")]
        public string Name { get; init; } = string.Empty;

        [Required(ErrorMessage = "A URL da aplicação é obrigatória.")]
        [Url(ErrorMessage = "A URL da aplicação deve ser válida.")]
        public string AppUrl { get; init; } = string.Empty;

    }
}
