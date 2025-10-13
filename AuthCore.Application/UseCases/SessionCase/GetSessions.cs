using AuthCore.Application.Models.Output;
using AuthCore.Application.Services.Interfaces;
using AuthCore.Application.UseCases.SessionCase.Interfaces;
using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Domain.Commons.Exceptions;
using AuthCore.Domain.Commons.Interfaces.Http;
using AuthCore.Domain.Commons.Interfaces.Security;

namespace AuthCore.Application.UseCases.SessionCase
{
    public sealed class GetSessions : IGetSessions
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IAccessToken _accessToken;
        private readonly ICookie _cookie;
        private readonly IEntropy _entropy;
        private readonly IOwnership _ownership;

        public GetSessions(
            ISessionRepository sessionRepository,
            IAccessToken accessToken,
            ICookie cookie,
            IEntropy entropy,
            IOwnership ownership)
        {
            _sessionRepository = sessionRepository;
            _accessToken = accessToken;
            _cookie = cookie;
            _entropy = entropy;
            _ownership = ownership;
        }

        public async Task<IEnumerable<SessionViewModel>> OnExecute()
        {
            var userId = _accessToken.Sub;
            var hashedSession = _entropy.Hash(_cookie.Session);

            var sessions = (await _sessionRepository.GetByUserId(userId)).ToList();
            if (sessions.Count == 0)
                throw new NotFoundException();

            _ownership.EnsureAll(sessions.Select(s => s.UserId));

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
