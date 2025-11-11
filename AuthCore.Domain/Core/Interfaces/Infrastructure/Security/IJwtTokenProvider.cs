using AuthCore.Domain.Aggregates.UserAggregate;

namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Security
{
    /// <summary>
    /// Define métodos para criar e ler informações de tokens JWT.
    /// </summary>
    public interface IJwtTokenProvider
    {
        /// <summary>
        /// Identificador do usuário associado ao token.
        /// </summary>
        Guid Sub { get; }

        /// <summary>
        /// Gera um novo token de acesso para o usuário informado.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="role">Função (role) do usuário.</param>
        string Generate(Guid userId);
    }
}
