using AuthCore.Domain.Aggregates.Notifications;

namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Security
{
    /// <summary>Define operações de mitigação de abuso para códigos de confirmação.</summary>
    public interface IConfirmCodeAbuseGuard
    {
        /// <summary>Operação para validar limite por IP.</summary>
        /// <param name="ipAddress">Endereço IP da requisição.</param>
        /// <param name="type">Tipo da notificação/código.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        Task EnsureIpAllowedAsync(string? ipAddress, NotificationType type, DateTime utcNow);

        /// <summary>Operação para validar e registrar envio para usuário.</summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="type">Tipo da notificação/código.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        Task CheckAndRegisterUserAsync(Guid userId, NotificationType type, DateTime utcNow);
    }
}
