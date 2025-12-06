namespace AuthCore.Domain.Aggregates.UserAggregate
{
    /// <summary>
    /// Define níveis básicos de acesso dos usuários.
    /// </summary>
    public enum Role
    {
        /// <summary>
        /// Concede permissões padrão ao usuário.
        /// </summary>
        User,

        /// <summary>
        /// Concede permissões administrativas.
        /// </summary>
        Admin,

        /// <summary>
        /// Concede todas as permissões do sistema.
        /// </summary>
        Owner
    }
}
