using System.ComponentModel;

namespace AuthCore.Domain.Aggregates.Users
{
    /// <summary>Define níveis básicos de acesso dos usuários.</summary>
    public enum Role
    {
        /// <summary>Usuário padrão.</summary>
        [Description("Usuário padrão.")]
        User = 1,

        /// <summary>Administrador.</summary>
        [Description("Administrador.")]
        Admin = 2,

        /// <summary>Proprietário.</summary>
        [Description("Proprietário.")]
        Owner = 3
    }
}
