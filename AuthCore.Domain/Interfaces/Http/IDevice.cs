using AuthCore.Domain.ValueObjects;

namespace AuthCore.Domain.Interfaces.Http
{
    public interface IDevice
    {
        DeviceInfo Device { get; }
    }
}
