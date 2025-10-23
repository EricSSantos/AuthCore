using AuthCore.Application.Models.Input;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Commons.Interfaces.Security.Hashing;
using AuthCore.Domain.Commons.Interfaces.Security.Jwt;

namespace AuthCore.Application.UseCases.UserCase
{
    public sealed class ChangePassword : IChangePassword
    {
        private readonly IUserRepository _userRepository;
        private readonly IBCrypt _bCrypt;
        private readonly IAccessToken _accessToken;

        public ChangePassword(
            IUserRepository userRepository,
            IBCrypt bCrypt,
            IAccessToken accessToken)
        {
            _userRepository = userRepository;
            _bCrypt = bCrypt;
            _accessToken = accessToken;
        }

        public async Task OnExecute(ChangePasswordInputModel input)
        {
            var user = await _userRepository.GetById(_accessToken.Sub);
            if (user is null)
                return;

            _bCrypt.isValid(input.CurrentPassword, user.Password.Value);

            user.ChangePassword(
                _bCrypt.Hash(input.NewPassword),
                input.NewPassword
            );

            _userRepository.Update(user);
            await _userRepository.SaveChanges();
        }
    }
}
