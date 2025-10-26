using AuthCore.Application.Models.Input;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using AuthCore.Domain.Aggregates.EmailAggregate;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Security;

namespace AuthCore.Application.UseCases.UserCase
{
    public sealed class AddUser : IAddUser
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEmailPublisher _emailPublisher;

        public AddUser(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IEmailPublisher emailPublisher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _emailPublisher = emailPublisher;
        }

        public async Task OnExecute(AddUserInputModel input)
        {
            if (await _userRepository.Exists(u => u.Email == input.Email))
                throw new ConflictException("E-mail já cadastrado.");

            Password.ValidateWithConfirmation(input.Password, input.ConfirmPassword);

            var user = User.Create(
                firstName:      input.FirstName,
                lastName:       input.LastName,
                email:          input.Email,
                passwordHash:   _passwordHasher.Hash(input.Password)
            );

            await _userRepository.Add(user);
            await _userRepository.SaveChanges();

            await SendWelcomeEmail(user.Email, user.FullName);
        }

        #region Helpers

        private async Task SendWelcomeEmail(string to, string fullName)
        {
            var email = Email.Create(
                to:         to,
                fullName:   fullName,
                type:       EmailType.Welcome
            );

            await _emailPublisher.Send(email);
        }

        #endregion
    }
}
