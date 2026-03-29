using AuthCore.Domain.Aggregates.Users.Interfaces;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Settings;

namespace AuthCore.Domain.Aggregates.Users.Policies
{
    /// <summary>Representa a política padrão de autenticação.</summary>
    public sealed class DefaultUserAuthenticationPolicy : IUserAuthenticationPolicy
    {
        private readonly int _maxAttempts;
        private readonly TimeSpan _lockDuration;

        /// <summary>Operação para criar instância de política.</summary>
        /// <param name="settings">Configurações de segurança.</param>
        public DefaultUserAuthenticationPolicy(SecuritySettings settings)
        {
            _maxAttempts = settings.LoginAttempts.MaxAttempts;
            _lockDuration = TimeSpan.FromMinutes(settings.LoginAttempts.LockDurationMinutes);
        }

        /// <summary>Operação para validar autenticação do usuário.</summary>
        /// <param name="user">Usuário a validar.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        public void EnsureCanAuthenticate(User user, DateTime utcNow)
        {
            if (!user.IsActiveAndVerified())
                throw new InvalidCredentialsException();
            if (user.LoginAttempts.IsLocked(utcNow))
                throw new AccountLockedException(user.LoginAttempts.GetLockMessage(utcNow)!);
        }

        /// <summary>Operação para registrar falha de autenticação.</summary>
        /// <param name="loginAttempts">Tentativas atuais.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        public LoginAttempts RegisterFailure(LoginAttempts loginAttempts, DateTime utcNow)
        {
            return loginAttempts.RegisterFailure(utcNow, _maxAttempts, _lockDuration);
        }
    }
}
