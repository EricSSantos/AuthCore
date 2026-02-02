using AuthCore.Application.Models.Responses;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using AuthCore.Domain.Aggregates.UserAggregate.Interfaces;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;

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

        public async Task<UserResponse> OnExecuteAsync()
        {
            var user = await _userRepository.GetByIdAsync(_jwtTokenProvider.Sub)
                ?? throw new NotFoundException("Usuário não encontrado.");

            if (!user.Active || !user.Verified)
                throw new ForbiddenException("A conta deste usuário está inativa.");

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email.Value,
                FullName = user.FullName,
                Role = user.Role.ToString()
            };
        }
    }
}
