using AuthCore.Domain.Aggregates.SessionAggregate;

namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Http
{
    /// <summary>
    /// Define acesso às informações do dispositivo do cliente.
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// Obtém os dados do dispositivo atual.
        /// </summary>
        DeviceInfo Device { get; }
    }
}
