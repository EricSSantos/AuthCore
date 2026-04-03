namespace MailCore.Service.Templates
{
    /// <summary>Template do e-mail de recuperação de senha.</summary>
    public sealed class ForgotPasswordEmailTemplate
    {
        private readonly TemplateRenderer _renderer;

        /// <summary>Inicializa o template de recuperação de senha.</summary>
        /// <param name="renderer">Renderizador de layout base.</param>
        public ForgotPasswordEmailTemplate(TemplateRenderer renderer)
        {
            _renderer = renderer;
        }

        /// <summary>Renderiza o HTML final de recuperação de senha.</summary>
        /// <param name="fullName">Nome do destinatário.</param>
        /// <param name="code">Código de recuperação.</param>
        public string Render(string fullName, int code)
        {
            var body = $@"
                <h2>Olá, {fullName}!</h2>
                <p>Recebemos uma solicitação para redefinir sua senha.</p>
                <div style=""text-align:center;margin:28px 0;"">
                    <div style=""display:inline-block;background:#7C3AED;color:#fff;padding:14px 26px;border-radius:8px;
                                 font-size:22px;letter-spacing:4px;font-weight:600;"">
                        {code}
                    </div>
                </div>
                <p style=""font-size:14px;color:#555;"">
                    Este código expira em alguns minutos. Se não foi você, ignore este e-mail.
                </p>";
            return _renderer.Wrap(body);
        }
    }
}
