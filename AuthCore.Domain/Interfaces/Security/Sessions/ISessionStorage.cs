using AuthCore.Domain.Entities;
using AuthCore.Domain.ValueObjects;

namespace AuthCore.Domain.Interfaces.Security.Sessions
{
    public interface ISessionStorage
    {
        Task<Session?> GetById(Guid id);
        Task<Session?> GetBySession(Guid userId, string rawSession);
        Task<Session?> GetByDevice(Guid userId, DeviceInfo device);
        Task Add(Session session);
        Task Update(Session session);
        Task Delete(Guid id);
    }
}
