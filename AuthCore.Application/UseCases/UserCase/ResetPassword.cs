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
            var user = await _userRepository.GetByEmail(input.Email)
                ?? throw new NotFoundException("Usuário não encontrado.");

            var confirmCode = await _confirmCodeRepository.Get(user.Id, CodeType.ForgotPassword)
                ?? throw new NotFoundException("Código nãod encontrado.");

            if (confirmCode.IsMatching(input.Code) || !user.IsActive())
                throw new BadRequestException("Código de verificação inválido.");

            user.ChangePassword(
                hashedPassword:     _bcrypt.Hash(input.NewPassword),
                password:           input.NewPassword,
                confirmPassword:    input.ConfirmNewPassword
            );

            _userRepository.Update(user);
            await _userRepository.SaveChanges();
        }
    }
}
