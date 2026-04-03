using AuthCore.Application.Models.Responses;
using AuthCore.Application.UseCases.Users.Interfaces;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;

namespace AuthCore.Application.UseCases.Users
{
    /// <summary>Representa caso de uso de obtenção do usuário atual.</summary>
    public sealed class GetCurrentUser : IGetCurrentUser
    {
        private readonly ISessionState _sessionState;

        /// <summary>Operação para criar instância de caso de uso.</summary>
        /// <param name="sessionState">Gerenciador de estado da sessão atual.</param>
        public GetCurrentUser(
            ISessionState sessionState)
        {
            _sessionState = sessionState;
        }

        /// <summary>Operação para retornar os dados do usuário atual.</summary>
        public async Task<UserResponse> OnExecuteAsync()
        {
            var user = await _sessionState.GetCurrentUser();
            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email.Value,
                FullName = user.FullName,
                Role = user.Role.ToString()
            };
        }
    }
}
