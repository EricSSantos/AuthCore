using AuthCore.Application.Models.Requests;
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

        public async Task OnExecute(ResetPasswordRequest request)
        {
            var user = await _userRepository.GetByEmail(request.Email)
                ?? throw new NotFoundException("Usuário não encontrado.");

            if (!user.IsActive())
                throw new ForbiddenException("Usuário inativo.");

            var confirm = await _confirmCodeRepository.Get(user.Id, CodeType.ForgotPassword)
                ?? throw new NotFoundException("Código de confirmação não encontrado");

            confirm.Matching(request.Code);
            await _confirmCodeRepository.Delete(user.Id, confirm.Type);

            Password.ValidateWithConfirmation(request.NewPassword, request.ConfirmNewPassword);

            user.ChangePassword(passwordHash: _passwordHasher.Hash(request.NewPassword));

            _userRepository.Update(user);
            await _userRepository.SaveChanges();
        }
    }
}
