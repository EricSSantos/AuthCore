using AuthCore.Domain.Entities;
using AuthCore.Domain.ValueObjects;

namespace AuthCore.Domain.Interfaces.Adapters.Sessions
{
    public interface ISessionAdapter
    {
        Task<Session?> GetBySid(Guid sid);
        Task<Session?> GetBySubAndDevice(Guid sub, DeviceInfo device);
        Task Add(Session session, TimeSpan? ttl = null);
        Task Delete(Guid sid);
    }
}
