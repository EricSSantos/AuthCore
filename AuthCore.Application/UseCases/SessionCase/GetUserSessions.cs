using AuthCore.Application.Models.Output;
using AuthCore.Application.UseCases.SessionCase.Interfaces;
using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Domain.Commons.Interfaces.Http;
using AuthCore.Domain.Commons.Interfaces.Security;
using System.Net;

namespace AuthCore.Application.UseCases.SessionCase
{
    public sealed class GetUserSessions : IGetUserSessions
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IAccessToken _accessToken;
        private readonly ICookie _cookie;
        private readonly IEntropy _entropy;

        public GetUserSessions(ISessionRepository sessionRepository, IAccessToken accessToken, ICookie cookie, IEntropy entropy)
        {
            _sessionRepository = sessionRepository;
            _accessToken = accessToken;
            _cookie = cookie;
            _entropy = entropy;
        }

        public async Task<IEnumerable<SessionViewModel>> OnExecute()
        {
            var sessions = await _sessionRepository.GetByUserId(_accessToken.Sub)
                ?? throw new Exception();

            return sessions.Select(SessionViewModel.FromEntity);
        }
    }
}
