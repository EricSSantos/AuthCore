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
            // Se o e-mail já estiver cadastrado, interrompe o processo silenciosamente.
            // Isso evita expor informações sobre contas existentes e protege contra
            // ataques de enumeração de e-mails válidos.
            if (await _userRepository.Exists(u => u.Email == input.Email))
                return;

            if (input.Password != input.ConfirmPassword)
                throw new DomainException("A confirmação da senha não corresponde.");

            Password.EnsureIsValid(input.Password);

            var hashedPwd = _bcrypt.Hash(input.Password);

            var user = User.Create(
                firstName: input.FirstName,
                lastName: input.LastName,
                email: input.Email,
                password: hashedPwd
            );

            await _userRepository.Add(user);
            await _userRepository.SaveChanges();

            var email = Email.Create(
                to: user.Email,
                fullName: user.FullName,
                type: EmailType.Welcome
            );

            await _emailService.Send(email);
        }
    }
}
