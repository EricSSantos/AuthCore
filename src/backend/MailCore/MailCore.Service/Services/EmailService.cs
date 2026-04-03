using MailCore.Service.Contracts;
using MailCore.Service.Settings;
using MailCore.Service.Templates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MailCore.Service.Services
{
    /// <summary>Responsável por montar e enviar e-mails via SMTP.</summary>
    public sealed class EmailService
    {
        private static readonly Regex EmailRegex = new(
            @"^[^\s@]+@[^\s@]+\.[^\s@]+$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        private readonly SmtpSettings _smtpSettings;
        private readonly ConfirmEmailTemplate _confirmEmailTemplate;
        private readonly WelcomeEmailTemplate _welcomeEmailTemplate;
        private readonly ForgotPasswordEmailTemplate _forgotPasswordEmailTemplate;
        private readonly ILogger<EmailService> _logger;

        /// <summary>Inicializa o serviço de envio de e-mails.</summary>
        public EmailService(
            IOptions<SmtpSettings> smtpOptions,
            ConfirmEmailTemplate confirmEmailTemplate,
            WelcomeEmailTemplate welcomeEmailTemplate,
            ForgotPasswordEmailTemplate forgotPasswordEmailTemplate,
            ILogger<EmailService> logger)
        {
            _smtpSettings = smtpOptions.Value;
            _confirmEmailTemplate = confirmEmailTemplate;
            _welcomeEmailTemplate = welcomeEmailTemplate;
            _forgotPasswordEmailTemplate = forgotPasswordEmailTemplate;
            _logger = logger;
        }

        /// <summary>Envia o e-mail recebido da fila.</summary>
        public async Task SendEmail(Email emailMessage)
        {
            ValidateRequest(emailMessage);
            var (subject, htmlContent) = BuildContent(emailMessage);

            _logger.LogInformation(
                "Preparando envio de e-mail. Id={Id}, Tipo={Type}, Destinatario={To}",
                emailMessage.Id,
                emailMessage.Type,
                emailMessage.To);

            using var mailMessage = BuildMailMessage(emailMessage.To, subject, htmlContent);
            using var smtpClient = BuildSmtpClient();

            _logger.LogInformation(
                "Conectando SMTP. Host={Host}, Port={Port}, Ssl={EnableSsl}, TimeoutSeconds={TimeoutSeconds}",
                _smtpSettings.Host,
                _smtpSettings.Port,
                _smtpSettings.EnableSsl,
                _smtpSettings.TimeoutSeconds);

            try
            {
                await smtpClient
                    .SendMailAsync(mailMessage)
                    .WaitAsync(TimeSpan.FromSeconds(_smtpSettings.TimeoutSeconds));
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(
                    ex,
                    "Timeout ao enviar e-mail via SMTP. Id={Id}, Host={Host}, Port={Port}",
                    emailMessage.Id,
                    _smtpSettings.Host,
                    _smtpSettings.Port);
                throw;
            }
            catch (SmtpException ex)
            {
                _logger.LogError(
                    ex,
                    "Falha SMTP ao enviar e-mail. Id={Id}, Host={Host}, Port={Port}",
                    emailMessage.Id,
                    _smtpSettings.Host,
                    _smtpSettings.Port);
                throw;
            }

            _logger.LogInformation(
                "E-mail enviado com sucesso. Id={Id}, Tipo={Type}, Destinatario={To}",
                emailMessage.Id,
                emailMessage.Type,
                emailMessage.To);
        }

        /// <summary>Valida os dados mínimos da mensagem de e-mail.</summary>
        private static void ValidateRequest(Email emailMessage)
        {
            if (emailMessage is null)
                throw new InvalidOperationException("A mensagem de e-mail não pode ser nula.");

            if (string.IsNullOrWhiteSpace(emailMessage.To))
                throw new InvalidOperationException("O destinatário do e-mail é obrigatório.");

            var recipient = emailMessage.To.Trim();
            if (!EmailRegex.IsMatch(recipient))
            {
                throw new InvalidOperationException(
                    $"O destinatário '{emailMessage.To}' é inválido.");
            }

            if (string.IsNullOrWhiteSpace(emailMessage.FullName))
                throw new InvalidOperationException("O nome do destinatário é obrigatório.");
        }

        /// <summary>Monta assunto e HTML conforme o tipo do e-mail.</summary>
        private (string Subject, string HtmlContent) BuildContent(Email emailMessage)
        {
            string subject;
            string body;

            switch (emailMessage.Type)
            {
                case EmailType.ConfirmEmail:
                    var confirmCode = ReadCodeFromPayload(emailMessage.Payload, "confirmação de e-mail");
                    subject = "Confirme seu e-mail";
                    body = _confirmEmailTemplate.Render(emailMessage.FullName, confirmCode);
                    break;

                case EmailType.Welcome:
                    subject = "Boas-vindas!";
                    body = _welcomeEmailTemplate.Render(emailMessage.FullName);
                    break;

                case EmailType.ForgotPassword:
                    var resetCode = ReadCodeFromPayload(emailMessage.Payload, "recuperação de senha");
                    subject = "Redefina sua senha";
                    body = _forgotPasswordEmailTemplate.Render(emailMessage.FullName, resetCode);
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Tipo de e-mail não suportado: {emailMessage.Type}. Id={emailMessage.Id}");
            }

            return (subject, body);
        }

        /// <summary>Lê o código numérico do payload da mensagem.</summary>
        private static int ReadCodeFromPayload(JsonElement? payload, string context)
        {
            if (payload is null || payload.Value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
                throw new InvalidOperationException($"O payload de {context} está ausente.");

            var payloadValue = payload.Value;
            if (payloadValue.ValueKind != JsonValueKind.Object)
                throw new InvalidOperationException($"O payload de {context} está em formato inválido.");

            if (TryReadCode(payloadValue, "code", out var code) || TryReadCode(payloadValue, "Code", out code))
            {
                if (code <= 0)
                    throw new InvalidOperationException($"O código de {context} deve ser maior que zero.");
                return code;
            }

            throw new InvalidOperationException($"O payload de {context} não possui o campo de código.");
        }

        /// <summary>Tenta ler o campo de código do payload.</summary>
        private static bool TryReadCode(JsonElement payload, string propertyName, out int code)
        {
            code = default;
            if (!payload.TryGetProperty(propertyName, out var codeElement))
                return false;

            if (codeElement.ValueKind == JsonValueKind.Number && codeElement.TryGetInt32(out var numericCode))
            {
                code = numericCode;
                return true;
            }

            if (codeElement.ValueKind == JsonValueKind.String &&
                int.TryParse(codeElement.GetString(), out var stringCode))
            {
                code = stringCode;
                return true;
            }

            return false;
        }

        /// <summary>Monta a mensagem SMTP final.</summary>
        private MailMessage BuildMailMessage(string destination, string subject, string htmlContent)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                Subject = subject,
                Body = htmlContent,
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(destination.Trim()));
            return message;
        }

        /// <summary>Cria um cliente SMTP com as configurações atuais.</summary>
        private SmtpClient BuildSmtpClient()
        {
            return new SmtpClient(_smtpSettings.Host)
            {
                Port = _smtpSettings.Port,
                Credentials = new NetworkCredential(_smtpSettings.User, _smtpSettings.Password),
                Timeout = _smtpSettings.TimeoutSeconds * 1000,
                EnableSsl = _smtpSettings.EnableSsl
            };
        }
    }
}
