using AuthCore.Domain.Aggregates.UserAggregate;

namespace AuthCore.Domain.Commons.Interfaces.Security
{
    public interface IAccessToken
    {
        Guid Sub { get; }
        Role Role { get; }
        string Generate(Guid userId, Role role);
    }
}
