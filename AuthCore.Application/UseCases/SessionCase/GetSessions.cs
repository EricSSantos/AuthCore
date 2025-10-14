using AuthCore.Application.Models.Output;
using AuthCore.Application.UseCases.SessionCase.Interfaces;
using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Domain.Commons.Exceptions;
using AuthCore.Domain.Commons.Interfaces.Http;
using AuthCore.Domain.Commons.Interfaces.Security.Hashing;
using AuthCore.Domain.Commons.Interfaces.Security.Jwt;

namespace AuthCore.Application.UseCases.SessionCase
{
    public sealed class GetSessions : IGetSessions
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IAccessToken _accessToken;
        private readonly ICookie _cookie;
        private readonly IEntropy _entropy;

        public GetSessions(
            ISessionRepository sessionRepository,
            IAccessToken accessToken,
            ICookie cookie,
            IEntropy entropy)
        {
            _sessionRepository = sessionRepository;
            _accessToken = accessToken;
            _cookie = cookie;
            _entropy = entropy;
        }

        public async Task<IEnumerable<SessionViewModel>> OnExecute()
        {
            var userId = _accessToken.Sub;
            var hashedSession = _entropy.Hash(_cookie.Session);

            var sessions = (await _sessionRepository.GetByUserId(userId)).ToList();
            if (sessions.Count == 0)
                throw new NotFoundException("Nenhuma sessão encontrada.");

            return sessions.Select(s => new SessionViewModel
            {
                Id = s.Id,
                IpAddress = s.DeviceInfo.Ip,
                Platform = s.DeviceInfo.Platform,
                Browser = s.DeviceInfo.Browser,
                CreatedAt = s.CreatedAt,
                IsCurrent = s.SessionHash == hashedSession
            });
        }
    }
}
