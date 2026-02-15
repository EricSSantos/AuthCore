using AuthCore.Domain.Aggregates.Users.Interfaces;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Settings;

namespace AuthCore.Domain.Aggregates.Users.Policies
{
    /// <summary>Representa a política padrão de autenticação.</summary>
    public sealed class DefaultUserAuthenticationPolicy : IUserAuthenticationPolicy
    {
        private readonly SecuritySettings _settings;

        #region Constructors

        /// <summary>Operação para criar instância de política.</summary>
        /// <param name="settings">Configurações de segurança.</param>
        public DefaultUserAuthenticationPolicy(SecuritySettings settings)
        {
            _settings = settings;
        }

        #endregion

        /// <summary>Operação para validar autenticação do usuário.</summary>
        /// <param name="user">Usuário a validar.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        public void EnsureCanAuthenticate(User user, DateTime utcNow)
        {
            if (!user.IsActiveAndVerified())
                throw new ForbiddenException("Usuário inativo ou não verificado.");

            if (user.LoginAttempts.IsLocked(utcNow))
                throw new UnauthorizedException(user.LoginAttempts.GetLockMessage(utcNow)!);
        }

        /// <summary>Operação para registrar falha de autenticação.</summary>
        /// <param name="loginAttempts">Tentativas atuais.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        public LoginAttempts RegisterFailure(LoginAttempts loginAttempts, DateTime utcNow)
        {
            var maxAttempts = _settings.LoginAttempts.MaxAttempts;
            var lockDuration = TimeSpan.FromMinutes(_settings.LoginAttempts.LockDurationMinutes);
            return loginAttempts.RegisterFailure(utcNow, maxAttempts, lockDuration);
        }
    }
}
