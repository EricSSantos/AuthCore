using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.Notifications.Interfaces;
using AuthCore.Domain.Aggregates.ConfirmCodes;
using AuthCore.Domain.Aggregates.ConfirmCodes.Interfaces;
using AuthCore.Domain.Aggregates.Notifications;
using AuthCore.Domain.Aggregates.Notifications.Interfaces;
using AuthCore.Domain.Aggregates.Users.Interfaces;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;

namespace AuthCore.Application.UseCases.Notifications
{
    /// <summary>Representa caso de uso de envio de notificações.</summary>
    public sealed class SendNotification : ISendNotification
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailSender _emailSender;
        private readonly IConfirmCodeRepository _confirmCodeRepository;
        private readonly IConfirmCodePolicy _confirmCodePolicy;
        private readonly INotificationPolicy _notificationPolicy;
        private readonly IConfirmCodeAbuseGuard _confirmCodeAbuseGuard;

        /// <summary>Operação para criar instância de caso de uso.</summary>
        /// <param name="userRepository">Repositório de usuários.</param>
        /// <param name="emailSender">Serviço de envio de e-mails.</param>
        /// <param name="confirmCodeRepository">Repositório de códigos de confirmação.</param>
        /// <param name="confirmCodePolicy">Política de códigos de confirmação.</param>
        /// <param name="notificationPolicy">Política de notificações.</param>
        /// <param name="confirmCodeAbuseGuard">Serviço de mitigação de abuso de códigos.</param>
        public SendNotification(
            IUserRepository userRepository,
            IEmailSender emailSender,
            IConfirmCodeRepository confirmCodeRepository,
            IConfirmCodePolicy confirmCodePolicy,
            INotificationPolicy notificationPolicy,
            IConfirmCodeAbuseGuard confirmCodeAbuseGuard)
        {
            _userRepository = userRepository;
            _emailSender = emailSender;
            _confirmCodeRepository = confirmCodeRepository;
            _confirmCodePolicy = confirmCodePolicy;
            _notificationPolicy = notificationPolicy;
            _confirmCodeAbuseGuard = confirmCodeAbuseGuard;
        }

        /// <summary>Operação para enviar o e-mail informado.</summary>
        /// <param name="request">Dados do e-mail.</param>
        public async Task OnExecuteAsync(SendNotificationRequest request)
        {
            var utcNow = DateTime.UtcNow;
            await _confirmCodeAbuseGuard.EnsureIpAllowedAsync(request.IpAddress, request.Type, utcNow);

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user is null)
                return;

            if (request.Type == NotificationType.ConfirmEmail && user.Verified)
                return;

            await _confirmCodeAbuseGuard.CheckAndRegisterUserAsync(user.Id, request.Type, utcNow);

            var codeValue = GetCodeValue(request.Type, utcNow);
            var payload = _notificationPolicy.CreatePayload(request.Type, codeValue);

            var email = EmailMessage.Create(
                to: user.Email.Value,
                fullName: user.FullName,
                type: request.Type,
                utcNow: utcNow,
                payload: payload
            );

            var code = CreateConfirmCode(email, codeValue, utcNow);

            await _confirmCodeRepository.SetAsync(user.Id, code);
            await _emailSender.SendAsync(email);
        }

        /// <summary>Operação para criar código de confirmação para o e-mail.</summary>
        /// <param name="email">Mensagem de e-mail a ser enviada.</param>
        /// <param name="codeValue">Valor do código gerado para a notificação.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        private ConfirmCode CreateConfirmCode(EmailMessage email, int? codeValue, DateTime utcNow)
        {
            switch (email.Type)
            {
                case NotificationType.ConfirmEmail:
                    return ConfirmCode.Create(codeValue!.Value, CodeType.ConfirmEmail, utcNow, utcNow.Add(_confirmCodePolicy.GetExpiration(CodeType.ConfirmEmail)));
                case NotificationType.ForgotPassword:
                    return ConfirmCode.Create(codeValue!.Value, CodeType.ForgotPassword, utcNow, utcNow.Add(_confirmCodePolicy.GetExpiration(CodeType.ForgotPassword)));
                default:
                    throw new BadRequestException("Tipo de e-mail inválido para geração de código.");
            }
        }

        /// <summary>Operação para obter o valor do código conforme o tipo de notificação.</summary>
        /// <param name="type">Tipo da notificação.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        private int? GetCodeValue(NotificationType type, DateTime utcNow)
        {
            switch (type)
            {
                case NotificationType.ConfirmEmail:
                    return _confirmCodePolicy.GenerateCode(CodeType.ConfirmEmail, utcNow);
                case NotificationType.ForgotPassword:
                    return _confirmCodePolicy.GenerateCode(CodeType.ForgotPassword, utcNow);
                default:
                    return null;
            }
        }
    }
}
