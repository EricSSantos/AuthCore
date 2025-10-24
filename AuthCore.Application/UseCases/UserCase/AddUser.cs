using AuthCore.Application.Models.Input;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using AuthCore.Domain.Aggregates.EmailAggregate;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Commons.Exceptions;
using AuthCore.Domain.Commons.Interfaces.Security.Hashing;

namespace AuthCore.Application.UseCases.UserCase
{
    public sealed class AddUser : IAddUser
    {
        private readonly IUserRepository _userRepository;
        private readonly IBCrypt _bcrypt;
        private readonly IEmailService _emailService;

        public AddUser(
            IUserRepository userRepository,
            IBCrypt bcrypt,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _bcrypt = bcrypt;
            _emailService = emailService;
        }

        public async Task OnExecute(AddUserInputModel input)
        {
            if (await _userRepository.Exists(u => u.Email == input.Email))
                throw new ConflictException("Já existe um usuário associado a este e-mail.");

            if (string.IsNullOrEmpty(input.ConfirmPassword))
                throw new BadRequestException("A confirmação de senha é obrigatória.");

            Password.Validate(input.Password, input.ConfirmPassword);

            var user = User.Create(
                firstName:  input.FirstName,
                lastName:   input.LastName,
                email:      input.Email,
                password:   _bcrypt.Hash(input.Password)
            );

            await _userRepository.Add(user);
            await _userRepository.SaveChanges();

            await SendWelcomeEmail(user.Email, user.FullName);
        }

        #region Private Methods

        private async Task SendWelcomeEmail(string to, string name)
        {
            var email = Email.Create(
                to:         to,
                fullName:   name,
                type:       EmailType.Welcome
            );

            await _emailService.Send(email);
        }

        #endregion
    }
}
