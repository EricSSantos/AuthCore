using AuthCore.Domain.Aggregates.Sessions;

namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Web
{
    /// <summary>Define operações para obtenção das informações do dispositivo do cliente.</summary>
    public interface IDevice
    {
        /// <summary>Operação para obter informações do dispositivo atual.</summary>
        DeviceInfo Get();
    }
}