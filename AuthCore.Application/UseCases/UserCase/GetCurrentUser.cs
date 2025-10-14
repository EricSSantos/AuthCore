using AuthCore.Application.Models.Output;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Commons.Exceptions;
using AuthCore.Domain.Commons.Interfaces.Security.Jwt;

namespace AuthCore.Application.UseCases.UserCase
{
    public sealed class GetCurrentUser : IGetCurrentUser
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccessToken _accessToken;

        public GetCurrentUser(
            IUserRepository userRepository,
            IAccessToken accessToken)
        {
            _userRepository = userRepository;
            _accessToken = accessToken;
        }

        public async Task<UserViewModel> OnExecute()
        {
            var user = await _userRepository.GetById(_accessToken.Sub)
                ?? throw new NotFoundException();

            return new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.FullName,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt
            };
        }
    }
}
