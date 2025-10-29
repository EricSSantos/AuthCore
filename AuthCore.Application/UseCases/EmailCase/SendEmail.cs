using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.EmailCase.Interface;
using AuthCore.Domain.Aggregates.ConfirmCodeAggregate;
using AuthCore.Domain.Aggregates.EmailAggregate;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Core.Exceptions;

namespace AuthCore.Application.UseCases.EmailCase
{
    public sealed class SendEmail : ISendEmail
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailPublisher _emailService;
        private readonly IConfirmCodeRepository _confirmCodeRepository;

        public SendEmail(
            IUserRepository userRepository,
            IEmailPublisher emailService,
            IConfirmCodeRepository confirmCodeRepository)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _confirmCodeRepository = confirmCodeRepository;
        }

        public async Task OnExecute(SendEmailRequest request)
        {
            var user = await _userRepository.GetByEmail(request.Email);
            if (user is null || !user.IsActive())
                return;

            var codeType = ResolveCodeType(request.Type);

            var email = Email.Create(
                to: user.Email,
                fullName: user.FullName,
                type: request.Type
            );

            var code = ConfirmCode.Create(codeType);

            await _confirmCodeRepository.Set(user.Id, code);
            await _emailService.Send(email);
        }

        #region Helpers

        private static CodeType ResolveCodeType(EmailType type)
        {
            switch (type)
            {
                case EmailType.ConfirmEmail:
                    return CodeType.ConfirmEmail;
                case EmailType.ForgotPassword:
                    return CodeType.ForgotPassword;
                default:
                    throw new BadRequestException("Tipo de e-mail inválido para geração de código.");
            }
        }

        #endregion
    }
}
