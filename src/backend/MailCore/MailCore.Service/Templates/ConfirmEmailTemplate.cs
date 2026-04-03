namespace MailCore.Service.Templates
{
    /// <summary>Template do e-mail de confirmação de conta.</summary>
    public sealed class ConfirmEmailTemplate
    {
        private readonly TemplateRenderer _renderer;

        /// <summary>Inicializa o template de confirmação.</summary>
        /// <param name="renderer">Renderizador de layout base.</param>
        public ConfirmEmailTemplate(TemplateRenderer renderer)
        {
            _renderer = renderer;
        }

        /// <summary>Renderiza o HTML final de confirmação.</summary>
        /// <param name="fullName">Nome do destinatário.</param>
        /// <param name="code">Código de confirmação.</param>
        public string Render(string fullName, int code)
        {
            var body = $@"
                <h2>Olá, {fullName}!</h2>
                <p>Estamos quase lá! Para ativar sua conta, utilize o código abaixo.</p>
                <div style=""text-align:center;margin:28px 0;"">
                    <div style=""display:inline-block;background:#7C3AED;color:#fff;padding:14px 26px;border-radius:8px;
                                 font-size:22px;letter-spacing:4px;font-weight:600;"">
                        {code}
                    </div>
                </div>
                <p style=""font-size:14px;color:#555;"">
                    Este código expira em alguns minutos. Caso não tenha solicitado o cadastro, ignore este e-mail com segurança.
                </p>";

            return _renderer.Wrap(body);
        }
    }
}
