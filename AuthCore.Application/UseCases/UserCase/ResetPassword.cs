using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using AuthCore.Domain.Aggregates.ConfirmCodeAggregate;
using AuthCore.Domain.Aggregates.ConfirmCodeAggregate.Interfaces;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Aggregates.UserAggregate.Interfaces;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;

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

        public async Task OnExecuteAsync(ResetPasswordRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user is null || !user.Active || !user.Verified)
                return;

            var confirm = await _confirmCodeRepository.GetAsync(user.Id, CodeType.ForgotPassword)
                ?? throw new NotFoundException("Código de confirmação não encontrado");

            confirm.Matching(request.Code);
            await _confirmCodeRepository.DeleteAsync(user.Id, confirm.Type);

            Password.ValidateWithConfirmation(request.NewPassword, request.ConfirmNewPassword);

            user.ChangePassword(passwordHash: _passwordHasher.Hash(request.NewPassword));

            await _userRepository.UpdateAsync(user);
        }
    }
}
