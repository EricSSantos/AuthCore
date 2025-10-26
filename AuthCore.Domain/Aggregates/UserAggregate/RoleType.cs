using System.ComponentModel;

namespace AuthCore.Domain.Aggregates.UserAggregate
{
    /// <summary>
    /// Define diferentes níveis de acesso de um usuário no sistema.
    /// </summary>
    public enum RoleType
    {
        [Description("Usuário padrão")]
        User,
        [Description("Administrador do sistema")]
        Admin
    }
}
