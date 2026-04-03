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

        [Range(1, int.MaxValue, ErrorMessage = "A versão do contrato da aplicação deve ser maior ou igual a 1.")]
        public int ContractVersion { get; init; } = 1;
    }
}
