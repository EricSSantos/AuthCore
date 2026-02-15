namespace AuthCore.Domain.Aggregates.Sessions.Results
{
    /// <summary>Representa resultado da criação de sessão.</summary>
    /// <param name="Session">Sessão criada.</param>
    /// <param name="RawSession">Valor bruto da sessão.</param>
    public sealed record SessionCreationResult(Session Session, string RawSession);
}
