namespace AuthCore.Domain.Aggregates.UserAggregate
{
    /// <summary>
    /// Define níveis básicos de acesso dos usuários.
    /// </summary>
    public enum Role
    {
        User = 1,
        Admin = 2,
        Owner = 3
    }
}
