namespace AuthCore.Domain.Aggregates.Sessions.Policies
{
    /// <summary>Define operações de política de sessão.</summary>
    public interface ISessionPolicy
    {
        /// <summary>Operação para selecionar sessões a revogar.</summary>
        /// <param name="sessions">Coleção de sessões do usuário.</param>
        IReadOnlyCollection<Session> SelectSessionsToRevoke(IReadOnlyCollection<Session> sessions);
    }
}
