using AuthCore.Application.Models.Input;
using AuthCore.Application.Models.Output;
using AuthCore.Domain.Commons.Exceptions;
using System.Text.RegularExpressions;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Commons.Interfaces.Security;

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

        public async Task<UserViewModel> OnExecute(AddUserInputModel input)
        {
            if (await _userRepository.Exists(u => u.Email == input.Email))
                throw new ConflictException("Endereço de e-mail já existe.");

            if (!IsStrong(input.Password))
                throw new DomainException("A senha não atende aos critérios de segurança.");

            if (input.Password != input.ConfirmPassword)
                throw new DomainException("A confirmação da senha não corresponde.");

            var password = _bcrypt.Hash(input.Password);
            var user = input.ToEntity(password);

            await _userRepository.Add(user);
            await _userRepository.SaveChanges();

            return UserViewModel.ToViewModel(user);
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
