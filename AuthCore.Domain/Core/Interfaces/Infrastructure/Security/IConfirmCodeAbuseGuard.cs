using AuthCore.Domain.Aggregates.Notifications;

namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Security
{
    /// <summary>Define operações de mitigação de abuso para códigos de confirmação.</summary>
    public interface IConfirmCodeAbuseGuard
    {
        /// <summary>Operação para validar limite por IP.</summary>
        Task EnsureIpAllowedAsync(string? ipAddress, NotificationType type, DateTime utcNow);

        /// <summary>Operação para validar e registrar envio para usuário.</summary>
        Task CheckAndRegisterUserAsync(Guid userId, NotificationType type, DateTime utcNow);

        /// <summary>Operação para registrar lockout após excesso de tentativas.</summary>
        Task RegisterLockoutAsync(Guid userId, NotificationType type, DateTime utcNow);
    }
}
