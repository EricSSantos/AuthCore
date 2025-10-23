using AuthCore.Application.Models.Input;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using AuthCore.Domain.Aggregates.ConfirmCodeAggregate;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Commons.Exceptions;
using AuthCore.Domain.Commons.Interfaces.Security.Hashing;

namespace AuthCore.Application.UseCases.UserCase
{
    public sealed class ResetPassword : IResetPassword
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfirmCodeRepository _confirmCodeRepository;
        private readonly IBCrypt _bcrypt;

        public ResetPassword(
            IUserRepository userRepository,
            IConfirmCodeRepository confirmCodeRepository,
            IBCrypt bcrypt)
        {
            _userRepository = userRepository;
            _confirmCodeRepository = confirmCodeRepository;
            _bcrypt = bcrypt;
        }

        public async Task OnExecute(ResetPasswordInputModel input)
        {
            var user = await _userRepository.GetByEmail(input.Email);
            if (user is null)
                return;

            var confirmCode = await _confirmCodeRepository.Get(user.Id, CodeType.ForgotPassword)
                ?? throw new DomainException("Código de confirmação inválido ou expirado.");

            if (confirmCode.Code != input.Code)
                throw new DomainException("O código informado é inválido.");

            user.ChangePassword(
                _bcrypt.Hash(input.NewPassword),
                input.NewPassword,
                input.ConfirmNewPassword
            );

            _userRepository.Update(user);
            await _userRepository.SaveChanges();
        }
    }
}
