using AuthCore.Application.Models.Input;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Security;

namespace AuthCore.Application.UseCases.UserCase
{
    public sealed class ChangePassword : IChangePassword
    {
        private readonly ISessionState _sessionState;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepository _userRepository;

        public ChangePassword(
            ISessionState sessionState,
            IPasswordHasher passwordHasher,
            IUserRepository userRepository)
        {
            _sessionState = sessionState;
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
        }

        public async Task OnExecute(ChangePasswordInputModel input)
        {
            var user = await _sessionState.GetCurrentUser();

            if (!_passwordHasher.IsValid(input.CurrentPassword, user.Password.Value))
                throw new BadRequestException("A senha atual está incorreta.");

            Password.ValidateWithConfirmation(input.NewPassword, input.ConfirmNewPassword);

            user.ChangePassword(_passwordHasher.Hash(input.NewPassword));

            _userRepository.Update(user);

            await _userRepository.SaveChanges();
        }
    }
}
