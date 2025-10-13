using AuthCore.Application.Models.Input;
using AuthCore.Application.Models.Output;
using AuthCore.Application.UseCases.UserCase.Interfaces;
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

        public AddUser(
            IUserRepository userRepository,
            IBCrypt bcrypt)
        {
            _userRepository = userRepository;
            _bcrypt = bcrypt;
        }

        public async Task OnExecute(AddUserInputModel input)
        {
            if (await _userRepository.Exists(u => u.Email == input.Email))
                throw new ConflictException("Endereço de e-mail já existe.");

            if (!IsStrong(input.Password))
                throw new DomainException("A senha não atende aos critérios de segurança.");

            if (input.Password != input.ConfirmPassword)
                throw new DomainException("A confirmação da senha não corresponde.");

            var password = _bcrypt.Hash(input.Password);

            var user = User.Create(
                firstName:  input.FirstName,
                lastName:   input.LastName,
                email:      input.Email,
                password:   password
            );

            await _userRepository.Add(user);
            await _userRepository.SaveChanges();
        }

        #region Private Methods

        private bool IsStrong(string password)
        {
            // 8 caracteres, 1 minúscula e 1 maiúscula
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z]).{8,}$");
            return regex.IsMatch(password);
        }

        #endregion
    }
}
