using AuthCore.Domain.Aggregates.UserAggregate;

namespace AuthCore.Domain.Commons.Interfaces.Security.Jwt
{
    public interface IAccessToken
    {
        /// <summary>
        /// Obtém o identificador do usuário associado ao token.
        /// </summary>
        Guid Sub { get; }

        /// <summary>
        /// Obtém a função (role) do usuário associada ao token.
        /// </summary>
        RoleType Role { get; }

        /// <summary>
        /// Gera um novo token de acesso para o usuário especificado.
        /// </summary>
        string Generate(Guid userId, RoleType role);
    }
}
