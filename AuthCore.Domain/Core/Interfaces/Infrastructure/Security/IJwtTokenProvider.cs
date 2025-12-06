using AuthCore.Domain.Aggregates.UserAggregate;

namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Security
{
    /// <summary>
    /// Define operações para criação e leitura de tokens JWT.
    /// </summary>
    public interface IJwtTokenProvider
    {
        /// <summary>
        /// Obtém o identificador do usuário do token.
        /// </summary>
        Guid Sub { get; }

        /// <summary>
        /// Gera um token de acesso para o usuário.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <returns>Token JWT gerado.</returns>
        string Generate(Guid userId);
    }
}
