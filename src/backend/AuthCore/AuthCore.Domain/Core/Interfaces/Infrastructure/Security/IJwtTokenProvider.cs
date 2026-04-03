namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Security
{
    /// <summary>Define operações para criação e leitura de tokens JWT.</summary>
    public interface IJwtTokenProvider
    {
        /// <summary>Operação para obter identificador do usuário autenticado a partir do token JWT.</summary>
        Guid Sub { get; }

        /// <summary>Operação para gerar token de acesso.</summary>
        /// <param name="userId">Identificador do usuário.</param>
        string Generate(Guid userId);
    }
}