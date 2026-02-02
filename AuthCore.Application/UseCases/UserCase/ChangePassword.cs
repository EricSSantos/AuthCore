using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using AuthCore.Domain.Aggregates.SessionAggregate.Interfaces;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Aggregates.UserAggregate.Interfaces;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;

namespace AuthCore.Application.UseCases.UserCase
{
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

            user.ChangePassword(_passwordHasher.Hash(request.NewPassword));

            await _userRepository.UpdateAsync(user);

            var sessions = await _sessionRepository.GetAllByUserIdAsync(user.Id);
            var deleteTasks = sessions.Select(s => _sessionRepository.DeleteAsync(s.Id));
            await Task.WhenAll(deleteTasks);
            _sessionState.ClearCookies();
        }
    }
}
