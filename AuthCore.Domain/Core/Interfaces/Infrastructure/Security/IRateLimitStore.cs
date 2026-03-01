namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Security
{
    /// <summary>Define operações de mitigação de abuso e rate limiting por chave.</summary>
    public interface IRateLimitStore
    {
        /// <summary>Operação para validar limite em janela fixa.</summary>
        /// <param name="key">Chave de partição do limite.</param>
        /// <param name="limit">Quantidade máxima permitida na janela.</param>
        /// <param name="window">Duração da janela fixa.</param>
        /// <param name="errorMessage">Mensagem de erro em caso de excesso.</param>
        Task EnsureFixedWindowAsync(string key, int limit, TimeSpan window, string errorMessage);

        /// <summary>Operação para validar cooldown por chave.</summary>
        /// <param name="key">Chave de partição do cooldown.</param>
        /// <param name="cooldown">Tempo mínimo entre operações.</param>
        /// <param name="errorMessage">Mensagem de erro quando o cooldown estiver ativo.</param>
        Task EnsureCooldownAsync(string key, TimeSpan cooldown, string errorMessage);
    }
}
