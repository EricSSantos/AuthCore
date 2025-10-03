using System.ComponentModel;

namespace AuthCore.Domain.Enums
{
    public enum Role
    {
        [Description("Usuário")]
        User,
        [Description("Administrador")]
        Admin,
        [Description("Proprietário")]
        Owner
    }
}
