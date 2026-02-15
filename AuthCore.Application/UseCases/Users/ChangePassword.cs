using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.Users.Interfaces;
using AuthCore.Domain.Aggregates.Sessions.Interfaces;
using AuthCore.Domain.Aggregates.Users;
using AuthCore.Domain.Aggregates.Users.Interfaces;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;

namespace AuthCore.Application.UseCases.Users
{
    /// <summary>Representa caso de uso de alteração de senha.</summary>
    public sealed class ChangePassword : IChangePassword
    {
        private readonly ISessionState _sessionState;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepository _userRepository;
        private readonly ISessionRepository _sessionRepository;

        public ChangePassword(
            ISessionState sessionState,
            IPasswordHasher passwordHasher,
            IUserRepository userRepository,
            ISessionRepository sessionRepository)
        {
            _sessionState = sessionState;
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
        }

        public async Task OnExecuteAsync(ChangePasswordRequest request)
        {
            var user = await _sessionState.GetCurrentUser();

            if (!_passwordHasher.IsValid(request.CurrentPassword, user.Password.Value))
                throw new BadRequestException("A senha atual está incorreta.");

            Password.ValidateWithConfirmation(request.NewPassword, request.ConfirmNewPassword);

            user.ChangePassword(_passwordHasher.Hash(request.NewPassword), DateTime.UtcNow);

            await _userRepository.UpdateAsync(user);

            var sessions = await _sessionRepository.GetAllByUserIdAsync(user.Id);
            var deleteTasks = sessions.Select(s => _sessionRepository.DeleteAsync(s.Id));
            await Task.WhenAll(deleteTasks);
            _sessionState.ClearCookies();
        }
    }
}
