using MailCore.Service.Settings;
using Microsoft.Extensions.Options;

namespace MailCore.Service.Templates
{
    /// <summary>Template do e-mail de boas-vindas.</summary>
    public sealed class WelcomeEmailTemplate
    {
        private readonly string _appUrl;
        private readonly TemplateRenderer _renderer;

        /// <summary>Inicializa o template de boas-vindas.</summary>
        /// <param name="applicationOptions">Configurações da aplicação.</param>
        /// <param name="renderer">Renderizador de layout base.</param>
        public WelcomeEmailTemplate(
            IOptions<ApplicationSettings> applicationOptions,
            TemplateRenderer renderer)
        {
            _appUrl = applicationOptions.Value.AppUrl;
            _renderer = renderer;
        }

        /// <summary>Renderiza o HTML final de boas-vindas.</summary>
        /// <param name="fullName">Nome do destinatário.</param>
        public string Render(string fullName)
        {
            var body = $@"
                <h2>Olá, {fullName}!</h2>
                <p>Boas-vindas! Sua conta está pronta para uso.</p>
                <div style=""text-align:center;margin:28px 0;"">
                    <a href=""{_appUrl}""
                       style=""display:inline-block;background:#7C3AED;color:#fff;text-decoration:none;
                              padding:12px 24px;border-radius:8px;font-size:15px;font-weight:600;"">
                        Acessar o app
                    </a>
                </div>";
            return _renderer.Wrap(body);
        }
    }
}
