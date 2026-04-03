using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.Users.Interfaces;
using AuthCore.Domain.Aggregates.ConfirmCodes;
using AuthCore.Domain.Aggregates.ConfirmCodes.Interfaces;
using AuthCore.Domain.Aggregates.Notifications;
using AuthCore.Domain.Aggregates.Notifications.Interfaces;
using AuthCore.Domain.Aggregates.Users.Interfaces;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Settings;
using Microsoft.Extensions.Logging;

namespace AuthCore.Application.UseCases.Users
{
    /// <summary>Representa caso de uso de confirmação de e-mail.</summary>
    public sealed class ConfirmEmail : IConfirmEmail
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfirmCodeRepository _confirmCodeRepository;
        private readonly IEmailSender _emailSender;
        private readonly INotificationPolicy _notificationPolicy;
        private readonly SecuritySettings _settings;
        private readonly ILogger<ConfirmEmail> _logger;

        /// <summary>Operação para criar instância de caso de uso.</summary>
        /// <param name="userRepository">Repositório de usuários.</param>
        /// <param name="confirmCodeRepository">Repositório de códigos de confirmação.</param>
        /// <param name="emailSender">Serviço de envio de e-mails.</param>
        /// <param name="notificationPolicy">Política de notificações.</param>
        /// <param name="settings">Configurações de segurança.</param>
        /// <param name="logger">Serviço de logging da operação.</param>
        public ConfirmEmail(
            IUserRepository userRepository,
            IConfirmCodeRepository confirmCodeRepository,
            IEmailSender emailSender,
            INotificationPolicy notificationPolicy,
            SecuritySettings settings,
            ILogger<ConfirmEmail> logger)
        {
            _userRepository = userRepository;
            _confirmCodeRepository = confirmCodeRepository;
            _emailSender = emailSender;
            _notificationPolicy = notificationPolicy;
            _settings = settings;
            _logger = logger;
        }

        /// <summary>Operação para confirmar o e-mail utilizando o código informado.</summary>
        /// <param name="request">Dados para confirmação.</param>
        public async Task OnExecuteAsync(ConfirmEmailRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user is null)
                return;

            var confirm = await _confirmCodeRepository.GetAsync(user.Id, CodeType.ConfirmEmail);
            if (confirm is null)
            {
                if (!user.IsActiveAndVerified())
                    throw new InvalidOrExpiredCodeException();
                return;
            }

            var utcNow = DateTime.UtcNow;
            if (confirm.IsExpired(utcNow))
            {
                await _confirmCodeRepository.DeleteAsync(user.Id, confirm.Type);
                throw new InvalidOrExpiredCodeException();
            }

            try
            {
                confirm.Matching(request.Code, utcNow, _settings.ConfirmCode.MaxAttempts);
            }
            catch
            {
                _logger.LogWarning("Falha na confirmação de e-mail para usuário {UserId}. Tentativas={Attempts}.", user.Id, confirm.Attempts);
                if (confirm.Attempts >= _settings.ConfirmCode.MaxAttempts)
                {
                    await _confirmCodeRepository.DeleteAsync(user.Id, confirm.Type);
                }
                else
                    await _confirmCodeRepository.SetAsync(user.Id, confirm);
                throw;
            }
            await _confirmCodeRepository.DeleteAsync(user.Id, confirm.Type);

            user.Confirm(utcNow);
            await _userRepository.UpdateAsync(user);

            var payload = _notificationPolicy.CreatePayload(NotificationType.Welcome, null);

            var email = EmailMessage.Create(
                to: user.Email.Value,
                fullName: user.FullName,
                type: NotificationType.Welcome,
                utcNow: utcNow,
                payload: payload
            );

            await _emailSender.SendAsync(email);
            _logger.LogInformation("E-mail confirmado para usuário {UserId}.", user.Id);
        }
    }
}
