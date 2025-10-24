using AuthCore.Application.Models.Input;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Commons.Exceptions;
using AuthCore.Domain.Commons.Interfaces.Security.Hashing;
using AuthCore.Domain.Commons.Interfaces.Security.Jwt;

namespace AuthCore.Application.UseCases.UserCase
{
    public sealed class ChangePassword : IChangePassword
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccessToken _accessToken;
        private readonly IBCrypt _bCrypt;
        
        public ChangePassword(
            IUserRepository userRepository,
            IAccessToken accessToken,
            IBCrypt bCrypt)
        {
            _userRepository = userRepository;
            _accessToken = accessToken;
            _bCrypt = bCrypt;
        }

        public async Task OnExecute(ChangePasswordInputModel input)
        {
            var user = await _userRepository.GetById(_accessToken.Sub)
                ?? throw new NotFoundException("Usuário não encontrado.");

            if (!_bCrypt.isValid(input.CurrentPassword, user.Password.Value) || !user.IsActive())
                throw new UnauthorizedException("A senha atual está incorreta.");

            user.ChangePassword(
                hashedPassword:     _bCrypt.Hash(input.NewPassword),
                password:           input.NewPassword,
                confirmPassword:    input.ConfirmNewPassword
            );

            _userRepository.Update(user);
            await _userRepository.SaveChanges();
        }
    }
}
