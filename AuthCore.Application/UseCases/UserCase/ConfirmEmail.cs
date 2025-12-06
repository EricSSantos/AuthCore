using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using AuthCore.Domain.Aggregates.ConfirmCodeAggregate;
using AuthCore.Domain.Aggregates.ConfirmCodeAggregate.Interfaces;
using AuthCore.Domain.Aggregates.MessagingAggregate;
using AuthCore.Domain.Aggregates.MessagingAggregate.Interfaces;
using AuthCore.Domain.Aggregates.UserAggregate.Interfaces;
using AuthCore.Domain.Core.Exceptions;

namespace AuthCore.Application.UseCases.UserCase
{
    public sealed class ConfirmEmail : IConfirmEmail
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfirmCodeRepository _confirmCodeRepository;
        private readonly IEmailPublisher _emailPublisher;

        public ConfirmEmail(
            IUserRepository userRepository,
            IConfirmCodeRepository confirmCodeRepository,
            IEmailPublisher emailPublisher)
        {
            _userRepository = userRepository;
            _confirmCodeRepository = confirmCodeRepository;
            _emailPublisher = emailPublisher;
        }

        public async Task OnExecuteAsync(ConfirmEmailRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email)
                ?? throw new NotFoundException("Usuário não encontrado");

            var confirm = await _confirmCodeRepository.GetAsync(user.Id, CodeType.ConfirmEmail)
                ?? throw new NotFoundException("Código de confirmação não encontrado");

            confirm.Matching(confirm.Code);
            await _confirmCodeRepository.DeleteAsync(user.Id, confirm.Type);

            user.Confirm();
            _userRepository.Update(user);
            await _userRepository.SaveChanges();

            var email = Messaging.Create(
                to: user.Email.Value,
                fullName: user.FullName,
                type: MessagingType.Welcome
            );

            await _emailPublisher.SendAsync(email);
        }
    }
}
