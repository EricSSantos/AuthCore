using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.EmailCase.Interface;
using AuthCore.Domain.Aggregates.ConfirmCodeAggregate;
using AuthCore.Domain.Aggregates.ConfirmCodeAggregate.Interfaces;
using AuthCore.Domain.Aggregates.MessagingAggregate;
using AuthCore.Domain.Aggregates.MessagingAggregate.Interfaces;
using AuthCore.Domain.Aggregates.MessagingAggregate.Payloads;
using AuthCore.Domain.Aggregates.UserAggregate.Interfaces;
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
            if (user is null)
                return;

            if (request.Type == MessagingType.ConfirmEmail && user.Verified)
                return;

            var email = Messaging.Create(
                to: user.Email.Value,
                fullName: user.FullName,
                type: request.Type
            );

            var code = CreateConfirmCode(email);

            await _confirmCodeRepository.Set(user.Id, code);
            await _emailService.Send(email);
        }

        #region Helpers

        private static ConfirmCode CreateConfirmCode(Messaging email)
        {
            switch (email.Type)
            {
                case MessagingType.ConfirmEmail:
                    var confirmPayload = email.GetPayload<ConfirmEmailPayload>();
                    return ConfirmCode.Create(confirmPayload.Code, CodeType.ConfirmEmail);
                case MessagingType.ForgotPassword:
                    var forgotPayload = email.GetPayload<ForgotPasswordPayload>();
                    return ConfirmCode.Create(forgotPayload.Code, CodeType.ForgotPassword);
                default:
                    throw new BadRequestException("Tipo de e-mail inválido para geração de código.");
            }
        }

        #endregion
    }
}
