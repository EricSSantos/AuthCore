using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using AuthCore.Domain.Aggregates.ConfirmCodeAggregate;
using AuthCore.Domain.Aggregates.ConfirmCodeAggregate.Interfaces;
using AuthCore.Domain.Aggregates.MessagingAggregate;
using AuthCore.Domain.Aggregates.MessagingAggregate.Interfaces;
using AuthCore.Domain.Aggregates.MessagingAggregate.Payloads;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Aggregates.UserAggregate.Interfaces;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;

namespace AuthCore.Application.UseCases.UserCase
{
    public sealed class AddUser : IAddUser
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfirmCodeRepository _confirmCodeRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEmailPublisher _emailPublisher;

        public AddUser(
            IUserRepository userRepository,
            IConfirmCodeRepository confirmCodeRepository,
            IPasswordHasher passwordHasher,
            IEmailPublisher emailPublisher)
        {
            _userRepository = userRepository;
            _confirmCodeRepository = confirmCodeRepository;
            _passwordHasher = passwordHasher;
            _emailPublisher = emailPublisher;
        }

        public async Task OnExecute(AddUserRequest request)
        {
            if (await _userRepository.Exists(u => u.Email.Value == request.Email))
                throw new ConflictException("E-mail já cadastrado.");

            Password.ValidateWithConfirmation(request.Password, request.ConfirmPassword);

            var user = User.Create(
                firstName: request.FirstName,
                lastName: request.LastName,
                email: request.Email,
                passwordHash: _passwordHasher.Hash(request.Password)
            );

            await _userRepository.Add(user);
            await _userRepository.SaveChanges();

            await SendEmail(user);
        }

        #region Helpers

        private async Task SendEmail(User user)
        {
            var email = Messaging.Create(
                to: user.Email.Value,
                fullName: user.FullName,
                type: MessagingType.ConfirmEmail
            );

            var payload = email.GetPayload<ConfirmEmailPayload>();

            var code = ConfirmCode.Create(
                code: payload.Code,
                type: CodeType.ConfirmEmail
            );

            await _confirmCodeRepository.Set(user.Id, code);
            await _emailPublisher.Send(email);
        }

        #endregion
    }
}
