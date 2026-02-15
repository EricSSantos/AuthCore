using AuthCore.Application.Models.Responses;
using AuthCore.Application.UseCases.Users.Interfaces;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;

namespace AuthCore.Application.UseCases.Users
{
    /// <summary>Representa caso de uso de obtenção do usuário atual.</summary>
    public sealed class GetCurrentUser : IGetCurrentUser
    {
        private readonly ISessionState _sessionState;

        public GetCurrentUser(
            ISessionState sessionState)
        {
            _sessionState = sessionState;
        }

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
