using AuthCore.Domain.ValueObjects;

namespace AuthCore.Domain.Interfaces.Adapters.Http
{
    public interface IDeviceAdapter
    {
        DeviceInfo GetDeviceInfo();
    }
}
