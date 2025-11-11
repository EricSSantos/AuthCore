using AuthCore.Domain.Aggregates.SessionAggregate;

namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Http
{
    /// <summary>
    /// Define propriedades para obter informações sobre o dispositivo do cliente.
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// Retorna as informações do dispositivo atual (IP, navegador, plataforma).
        /// </summary>
        DeviceInfo Device { get; }
    }
}
