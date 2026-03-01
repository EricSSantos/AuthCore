using System.Security.Cryptography;
using AuthCore.Domain.Aggregates.ConfirmCodes;
using AuthCore.Domain.Aggregates.ConfirmCodes.Interfaces;
using AuthCore.Domain.Core.Settings;

namespace AuthCore.Domain.Aggregates.ConfirmCodes.Policies
{
    /// <summary>Representa a política padrão de códigos de confirmação.</summary>
    public sealed class DefaultConfirmCodePolicy : IConfirmCodePolicy
    {
        #region Constants

        private const int CODE_MIN = DigitCode.MIN_VALUE;
        private const int CODE_MAX_EXCLUSIVE = DigitCode.MAX_VALUE + 1;

        #endregion

        private readonly SecuritySettings _settings;

        #region Constructors

        /// <summary>Operação para criar instância de política.</summary>
        /// <param name="settings">Configurações de segurança.</param>
        public DefaultConfirmCodePolicy(SecuritySettings settings)
        {
            _settings = settings;
        }

        #endregion

        /// <summary>Operação para gerar código de confirmação.</summary>
        /// <param name="type">Tipo do código.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        public int GenerateCode(CodeType type, DateTime utcNow)
        {
            return RandomNumberGenerator.GetInt32(CODE_MIN, CODE_MAX_EXCLUSIVE);
        }

        /// <summary>Operação para obter expiração do código.</summary>
        /// <param name="type">Tipo do código.</param>
        public TimeSpan GetExpiration(CodeType type)
        {
            return TimeSpan.FromMinutes(_settings.ConfirmCode.ExpiresInMinutes);
        }
    }
}
