using AuthCore.Application.Models.Input;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using AuthCore.Domain.Aggregates.ConfirmCodeAggregate;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Security;

namespace AuthCore.Application.UseCases.UserCase
{
    public sealed class ResetPassword : IResetPassword
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfirmCodeRepository _confirmCodeRepository;
        private readonly IPasswordHasher _passwordHasher;

        public ResetPassword(
            IUserRepository userRepository,
            IConfirmCodeRepository confirmCodeRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _confirmCodeRepository = confirmCodeRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task OnExecute(ResetPasswordInputModel input)
        {
            var user = await _userRepository.GetByEmail(input.Email)
                ?? throw new NotFoundException("Usuário não encontrado.");

            if (!user.IsActive())
                throw new ForbiddenException("Usuário inativo.");

            var confirmCode = await _confirmCodeRepository.Get(user.Id, CodeType.ForgotPassword)
                ?? throw new NotFoundException("Código de verificação não encontrado.");

            if (!confirmCode.Matching(input.Code))
                throw new BadRequestException("Código inválido ou expirado.");

            Password.ValidateWithConfirmation(input.NewPassword, input.ConfirmNewPassword);

            user.ChangePassword(passwordHash: _passwordHasher.Hash(input.NewPassword));

            _userRepository.Update(user);
            await _userRepository.SaveChanges();
        }
    }
}
