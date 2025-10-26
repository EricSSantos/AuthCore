using AuthCore.Application.Models.Output;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Security;

namespace AuthCore.Application.UseCases.UserCase
{
    public sealed class GetCurrentUser : IGetCurrentUser
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenProvider _jwtTokenProvider;

        public GetCurrentUser(
            IUserRepository userRepository,
            IJwtTokenProvider jwtTokenProvider)
        {
            _userRepository = userRepository;
            _jwtTokenProvider = jwtTokenProvider;
        }

        public async Task<UserViewModel> OnExecute()
        {
            var user = await _userRepository.GetById(_jwtTokenProvider.Sub)
                ?? throw new NotFoundException("Usuário não encontrado.");

            if (!user.IsActive())
                throw new ForbiddenException("A conta deste usuário está inativa.");

            return new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString()
            };
        }
    }
}
