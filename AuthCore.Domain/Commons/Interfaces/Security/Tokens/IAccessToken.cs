using AuthCore.Domain.Enums;

namespace AuthCore.Domain.Commons.Interfaces.Security.Tokens
{
    public interface IAccessToken
    {
        Guid Sub { get; }
        Role Role { get; }
        string Generate(Guid userId, Role role);
    }
}
