using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using AuthCore.Domain.Aggregates.ConfirmCodeAggregate;
using AuthCore.Domain.Aggregates.EmailAggregate;
using AuthCore.Domain.Aggregates.EmailAggregate.Payloads;
using AuthCore.Domain.Aggregates.UserAggregate;

namespace AuthCore.Application.UseCases.UserCase
{
    public sealed class ForgotPassword : IForgotPassword
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailPublisher _emailService;
        private readonly IConfirmCodeRepository _confirmCodeRepository;

        public ForgotPassword(
            IUserRepository userRepository,
            IEmailPublisher emailService,
            IConfirmCodeRepository confirmCodeRepository)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _confirmCodeRepository = confirmCodeRepository;
        }

        public async Task OnExecute(ForgotPasswordRequest request)
        {
            // Busca silenciosa, não revela se o e-mail existe
            var user = await _userRepository.GetByEmail(request.Email);
            if (user is null || !user.IsActive())
                return;

            var email = Email.Create(
                to: user.Email,
                fullName: user.FullName,
                type: EmailType.ForgotPassword
            );

            var payload = email.GetPayload<ForgotPasswordPayload>();

            var code = ConfirmCode.Create(
                code: payload.Code,
                type: CodeType.ForgotPassword
            );

            await _confirmCodeRepository.Set(user.Id, code);
            await _emailService.Send(email);
        }
    }
}
