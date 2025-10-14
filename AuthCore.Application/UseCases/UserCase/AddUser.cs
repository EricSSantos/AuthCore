using AuthCore.Application.Models.Input;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using AuthCore.Domain.Aggregates.EmailAggregate;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Commons.Exceptions;
using AuthCore.Domain.Commons.Interfaces.Security;
using System.Text.RegularExpressions;

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

        // TODO: Avaliar a criação de ValueObjects para Email e Password,
        // garantindo as validações e encapsulamento das regras no domínio.
        public async Task OnExecute(AddUserInputModel input)
        {
            if (await _userRepository.Exists(u => u.Email == input.Email))
                throw new ConflictException("Endereço de e-mail já cadastrado.");

            if (!IsStrong(input.Password))
                throw new DomainException("A senha não atende aos critérios de segurança.");

            if (input.Password != input.ConfirmPassword)
                throw new DomainException("A confirmação da senha não corresponde.");

            var password = _bcrypt.Hash(input.Password);

            var user = User.Create(
                firstName: input.FirstName,
                lastName: input.LastName,
                email: input.Email,
                password: password
            );

            await _userRepository.Add(user);
            await _userRepository.SaveChanges();

            // TESTE: Dispara e-mail de boas-vindas.
            await _emailService.Send(
                to: user.Email,
                fullName: user.FullName,
                type: EmailType.Welcome
            );
        }

        #region Private Methods

        private static bool IsStrong(string password)
        {
            // Pelo menos 8 caracteres, contendo letras minúsculas e maiúsculas
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z]).{8,}$");
            return regex.IsMatch(password);
        }

        #endregion
    }
}
