using System.ComponentModel;

namespace AuthCore.Domain.Aggregates.UserAggregate
{
    public enum RoleType
    {
        [Description("Usuário")]
        User,
        [Description("Administrador")]
        Admin,
        [Description("Proprietário")]
        Owner
    }
}
