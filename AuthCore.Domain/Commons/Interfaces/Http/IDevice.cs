using AuthCore.Domain.Aggregates.SessionAggregate;

namespace AuthCore.Domain.Commons.Interfaces.Http
{
    public interface IDevice
    {
        /// <summary>
        /// Obtém as informações do dispositivo da requisição atual.
        /// </summary>
        DeviceInfo Device { get; }
    }
}
