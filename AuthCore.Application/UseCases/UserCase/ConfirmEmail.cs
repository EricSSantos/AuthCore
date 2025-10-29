using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using AuthCore.Domain.Aggregates.ConfirmCodeAggregate;
using AuthCore.Domain.Aggregates.EmailAggregate;
using AuthCore.Domain.Aggregates.UserAggregate;
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

        public async Task OnExecute(ConfirmEmailRequest request)
        {
            var user = await _userRepository.GetByEmail(request.Email)
                ?? throw new NotFoundException("Usuário não encontrado");

            var confirm = await _confirmCodeRepository.Get(user.Id, CodeType.ConfirmEmail)
                ?? throw new NotFoundException("Código de confirmação não encontrado");

            // TODO: Criar um fluco de expurgo de códigos usados ou expirados
            confirm.Matching(confirm.Code);

            user.Confirm();
            _userRepository.Update(user);
            await _userRepository.SaveChanges();

            var email = Email.Create(
                to: user.Email,
                fullName: user.FullName,
                type: EmailType.Welcome
            );

            await _emailPublisher.Send(email);
        }
    }
}
