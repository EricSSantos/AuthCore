using AuthCore.Application.Models.Input;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using AuthCore.Domain.Aggregates.ConfirmCodeAggregate;
using AuthCore.Domain.Aggregates.EmailAggregate;
using AuthCore.Domain.Aggregates.EmailAggregate.Payloads;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Commons.Exceptions;

namespace AuthCore.Application.UseCases.UserCase
{
    public sealed class ForgotPassword : IForgotPassword
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IConfirmCodeRepository _confirmCodeRepository;

        public ForgotPassword(
            IUserRepository userRepository,
            IEmailService emailService,
            IConfirmCodeRepository confirmCodeRepository)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _confirmCodeRepository = confirmCodeRepository;
        }

        public async Task OnExecute(ForgotPasswordInputModel input)
        {
            // Busca silenciosa, não revela se o e-mail existe
            var user = await _userRepository.GetByEmail(input.Email);
            if (user is null || !user.IsActive())
                return;

            var email = Email.Create(
                to:         user.Email,
                fullName:   user.FullName,
                type:       EmailType.ForgotPassword
            );

            if (email.Payload is not ForgotPasswordPayload payload)
                throw new BadRequestException("Falha ao gerar o código de recuperação de senha.");

            var code = ConfirmCode.Create(
                code:   payload.Code,
                type:   CodeType.ForgotPassword
            );

            await _confirmCodeRepository.Set(user.Id, code);
            await _emailService.Send(email);
        }
    }
}
