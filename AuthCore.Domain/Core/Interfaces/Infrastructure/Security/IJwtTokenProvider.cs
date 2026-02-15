using AuthCore.Domain.Aggregates.Users;

namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Security
{
    /// <summary>Define operações para criação e leitura de tokens JWT.</summary>
    public interface IJwtTokenProvider
    {
        Guid Sub { get; }

        /// <summary>Operação para gerar token de acesso.</summary>
        /// <param name="userId">Identificador do usuário.</param>
        string Generate(Guid userId);
    }
}
