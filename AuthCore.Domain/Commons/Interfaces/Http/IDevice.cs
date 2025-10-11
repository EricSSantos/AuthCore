using AuthCore.Domain.Aggregates.SessionAggregate;

namespace AuthCore.Domain.Commons.Interfaces.Http
{
    public interface IDevice
    {
        DeviceInfo Device { get; }
    }
}
