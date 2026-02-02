namespace AuthCore.Domain.Aggregates.SessionAggregate
{
    /// <summary>
    /// Resultado da criacao de uma sessao com o valor bruto para cookie.
    /// </summary>
    public sealed record SessionCreationResult(Session Session, string RawSession);
}
