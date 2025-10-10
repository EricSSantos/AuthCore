using AuthCore.Domain.Entities.ValueObjects;

namespace AuthCore.Domain.Commons.Interfaces.Http
{
    public interface IDevice
    {
        DeviceInfo Device { get; }
    }
}
