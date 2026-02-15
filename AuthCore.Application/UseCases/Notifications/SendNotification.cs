using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.Notifications.Contracts;
using AuthCore.Domain.Aggregates.ConfirmCodes;
using AuthCore.Domain.Aggregates.ConfirmCodes.Contracts;
using AuthCore.Domain.Aggregates.ConfirmCodes.Policies;
using AuthCore.Domain.Aggregates.Notifications;
using AuthCore.Domain.Aggregates.Notifications.Contracts;
using AuthCore.Domain.Aggregates.Notifications.Policies;
using AuthCore.Domain.Aggregates.Users.Contracts;
using AuthCore.Domain.Core.Exceptions;

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

        public SendNotification(
            IUserRepository userRepository,
            IEmailSender emailSender,
            IConfirmCodeRepository confirmCodeRepository,
            IConfirmCodePolicy confirmCodePolicy,
            INotificationPolicy notificationPolicy)
        {
            _userRepository = userRepository;
            _emailSender = emailSender;
            _confirmCodeRepository = confirmCodeRepository;
            _confirmCodePolicy = confirmCodePolicy;
            _notificationPolicy = notificationPolicy;
        }

        public async Task OnExecuteAsync(SendNotificationRequest request)
        {
            var utcNow = DateTime.UtcNow;
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user is null)
                return;

            if (request.Type == NotificationType.ConfirmEmail && user.Verified)
                return;

            var codeValue = GetCodeValue(request.Type, utcNow);
            var email = Notification.Create(
                to: user.Email.Value,
                fullName: user.FullName,
                type: request.Type,
                utcNow: utcNow,
                code: codeValue,
                notificationPolicy: _notificationPolicy
            );

            var code = CreateConfirmCode(email, codeValue, utcNow);

            await _confirmCodeRepository.SetAsync(user.Id, code);
            await _emailSender.SendAsync(email);
        }

        #region Helpers

        private ConfirmCode CreateConfirmCode(Notification email, int? codeValue, DateTime utcNow)
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

        #endregion
    }
}
