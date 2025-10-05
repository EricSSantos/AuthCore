using AuthCore.Domain.Entities;
using AuthCore.Domain.ValueObjects;

namespace AuthCore.Domain.Interfaces.Adapters.Sessions
{
    public interface ISessionAdapter
    {
        Task<Session?> GetBySid(Guid sid);
        Task<Session?> GetBySubAndDevice(Guid sub, DeviceInfo device);
        Task<Session> Validate(Guid sessionId, Guid userId);
        Task Add(Session session);
        Task Update(Session session);
        Task Delete(Guid sid);
    }
}
