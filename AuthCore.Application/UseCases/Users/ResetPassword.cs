using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.Users.Interfaces;
using AuthCore.Domain.Aggregates.ConfirmCodes;
using AuthCore.Domain.Aggregates.ConfirmCodes.Interfaces;
using AuthCore.Domain.Aggregates.Notifications;
using AuthCore.Domain.Aggregates.Users;
using AuthCore.Domain.Aggregates.Users.Interfaces;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;
using AuthCore.Domain.Core.Settings;
using Microsoft.Extensions.Logging;

namespace AuthCore.Application.UseCases.Users
{
    /// <summary>Representa caso de uso de redefinição de senha.</summary>
    public sealed class ResetPassword : IResetPassword
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfirmCodeRepository _confirmCodeRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly SecuritySettings _settings;
        private readonly ILogger<ResetPassword> _logger;
        private readonly IConfirmCodeAbuseGuard _confirmCodeAbuseGuard;

        public ResetPassword(
            IUserRepository userRepository,
            IConfirmCodeRepository confirmCodeRepository,
            IPasswordHasher passwordHasher,
            SecuritySettings settings,
            ILogger<ResetPassword> logger,
            IConfirmCodeAbuseGuard confirmCodeAbuseGuard)
        {
            _userRepository = userRepository;
            _confirmCodeRepository = confirmCodeRepository;
            _passwordHasher = passwordHasher;
            _settings = settings;
            _logger = logger;
            _confirmCodeAbuseGuard = confirmCodeAbuseGuard;
        }

        public async Task OnExecuteAsync(ResetPasswordRequest request)
        {
            var utcNow = DateTime.UtcNow;
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user is null || !user.IsActiveAndVerified())
                return;

            var confirm = await _confirmCodeRepository.GetAsync(user.Id, CodeType.ForgotPassword)
                ?? throw new InvalidOrExpiredCodeException();

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
                _logger.LogWarning("Falha na redefinição de senha para usuário {UserId}. Tentativas={Attempts}.", user.Id, confirm.Attempts);
                if (confirm.Attempts >= _settings.ConfirmCode.MaxAttempts)
                {
                    await _confirmCodeAbuseGuard.RegisterLockoutAsync(user.Id, NotificationType.ForgotPassword, utcNow);
                    await _confirmCodeRepository.DeleteAsync(user.Id, confirm.Type);
                }
                else
                    await _confirmCodeRepository.SetAsync(user.Id, confirm);
                throw;
            }
            await _confirmCodeRepository.DeleteAsync(user.Id, confirm.Type);

            Password.ValidateWithConfirmation(request.NewPassword, request.ConfirmNewPassword);

            user.ChangePassword(passwordHash: _passwordHasher.Hash(request.NewPassword), utcNow);

            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("Senha redefinida para usuário {UserId}.", user.Id);
        }
    }
}
