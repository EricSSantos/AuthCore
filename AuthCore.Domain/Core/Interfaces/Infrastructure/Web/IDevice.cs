using AuthCore.Domain.Aggregates.Sessions;

namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Web
{
    /// <summary>Define acesso às informações do dispositivo do cliente.</summary>
    public interface IDevice
    {
        DeviceInfo Device { get; }
    }
}
